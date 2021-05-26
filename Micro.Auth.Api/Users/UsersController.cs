using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Micro.Auth.Api.Internal.UserData.Extensions;
using Micro.Auth.Api.Users.ViewModels;
using Micro.Auth.Business.Measurements;
using Micro.Auth.Business.Users;
using Micro.Auth.Business.Users.Exceptions;
using Micro.Auth.Storage;
using Micro.Mails.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly IMetrics _metrics;

        public UsersController(IUserRepository userRepository, IUserService userService, ILogger<UsersController> logger, IMetrics metrics)
        {
            _userRepository = userRepository;
            _userService = userService;
            _logger = logger;
            _metrics = metrics;
        }

        [HttpGet("findByUsername/{username}")]
        [ProducesResponseType(typeof(FindByUsernameResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<FindByUsernameResponse>> FindByUsername(string username)
        {
            try
            {
                var user = await _userRepository.FindByUsername(username);
                _metrics.UsersControllerMetrics().MarkFindUserByUsername();
                return Ok(new FindByUsernameResponse
                {
                    Available = user == null,
                    Username = username,
                });
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkFindUserException(e.GetType().FullName);
                _logger.LogError("caught exception", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpGet("findByEmail/{email}")]
        [ProducesResponseType(typeof(FindByEmailResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<FindByUsernameResponse>> FindByEmail(string email)
        {
            try
            {
                var user = await _userRepository.FindByEmail(email);
                _metrics.UsersControllerMetrics().MarkFindUserByEmail();
                return Ok(new FindByEmailResponse
                {
                    Available = user == null,
                    Email = email,
                });
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkFindUserException(e.GetType().FullName);
                _logger.LogError("caught exception", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("activation/sendEmail")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendActivationEmail(SendActivationEmailRequest request)
        {
            try
            {
                await _metrics.UsersControllerMetrics()
                    .RecordTimeToSendActivationEmail(async () =>
                        await _userService.SendActivationEmail(request.Login));
                return Accepted();
            }
            catch (UserNotFoundException)
            {
                _metrics.UsersControllerMetrics().MarkUserNotFoundActivation();
                return NotFound(new ProblemDetails {Type = "NotFound", Title = "user not found"});
            }
            catch (EmailSendingFailureException e)
            {
                _metrics.UsersControllerMetrics().MarkEmailSendingFailure();
                _metrics.UsersControllerMetrics().MarkExceptionActivation(e.GetType().FullName);
                _logger.LogWarning(e, "failed to send activation email");
                return BadRequest(new ProblemDetails {Title = "failed to send email"});
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkExceptionActivation(e.GetType().FullName);
                _logger.LogCritical(e, "unexpected error while sending activation email");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("activation/confirm")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            try
            {
                await _metrics.UsersControllerMetrics()
                    .RecordTimeToConfirmEmail(async () =>  await _userService.ConfirmEmail(request));
                _metrics.UsersControllerMetrics().MarkSuccessfulConfirmation();
                return Accepted();
            }
            catch (UserNotFoundException)
            {
                _logger.LogInformation($"user not found {request.Login}");
                _metrics.UsersControllerMetrics().MarkUserNotFoundActivation();
                return NotFound(new ProblemDetails {Type = "NotFound", Title = "user not found"});
            }
            catch (EmailConfirmationFailedException e)
            {
                _logger.LogInformation("EmailConfirmationFailed", e);
                _metrics.UsersControllerMetrics().MarkFailedToConfirmActivation();
                return Unauthorized(new ProblemDetails {Title = "failed to confirm"});
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkExceptionActivation(e.GetType().FullName);
                _logger.LogCritical(e, "unexpected error during email confirmation");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("password/requestReset")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordReset request)
        {
            try
            {
                await _metrics.UsersControllerMetrics().MeasureTimeToSendPasswordResetEmail(async () =>
                    await _userService.RequestPasswordReset(request.Login));
                return Accepted();
            }
            catch (UserNotFoundException)
            {
                _metrics.UsersControllerMetrics().MarkPasswordResetUserNotFound();
                return NotFound(new ProblemDetails
                {
                    Title = "user not found"
                });
            }
            catch (EmailSendingFailureException e)
            {
                _logger.LogError("error sending email", e);
                _metrics.UsersControllerMetrics().MarkEmailSendingFailure();
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("password/reset")]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                await _metrics.UsersControllerMetrics().MeasureTimeToResetPassword(async () =>
                    await _userService.ResetPassword(request));
                return Accepted();
            }
            catch (UserNotFoundException e)
            {
                _logger.LogWarning("user not found while resetting password: ", e);
                _metrics.UsersControllerMetrics().MarkPasswordResetUserNotFound();
                return NotFound(new ProblemDetails {Type = "NotFound", Title = "user not found"});
            }
            catch (PasswordResetFailedException e)
            {
                _logger.LogWarning("failed resetting password", e);
                _metrics.UsersControllerMetrics().MarkFailedToResetPassword();
                return Unauthorized(new ProblemDetails {Title = "failed to reset"});
            }
            catch (Exception e)
            {
                _logger.LogError("caught exception", e);
                _metrics.UsersControllerMetrics().MarkResetPasswordException(e.GetType().FullName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("password/change")]
        [Authorize]
        [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<ProblemDetails>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var result = await _userService.ChangePassword(this.GetUserId(), request);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(",", result.Errors.Select(x => x.Code).OrderBy(x => x));
                    _metrics.UsersControllerMetrics().MarkPasswordChangeFailure(errorMessage);
                    return BadRequest(result.Errors);
                }

                _metrics.UsersControllerMetrics().MarkPasswordChangeSuccess();
                return Accepted();
            }
            catch (UserNotFoundException e)
            {
                _metrics.UsersControllerMetrics().MarkPasswordChangeUserNotFound();
                _logger.LogError(e, "impossible happened, user not found", this.GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkPasswordChangeException(e.GetType().FullName);
                _logger.LogWarning(e, "caught exception");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "unknown error"
                });
            }
        }
    }
}
