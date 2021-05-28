using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using App.Metrics;
using Micro.Auth.Api.Authentication.Exceptions;
using Micro.Auth.Api.Authentication.ViewModels;
using Micro.Auth.Api.Internal.UserData.Extensions;
using Micro.Auth.Api.Internal.ValidationAttributes;
using Micro.Auth.Business.Internal.Measurements;
using Micro.Auth.Business.Sessions;
using Micro.Auth.Business.Users;
using Micro.Auth.Storage.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IMetrics _metrics;
        private readonly ISessionService _sessionService;

        public SessionController(ILogger<SessionController> logger, IMetrics metrics, ISessionService sessionService)
        {
            _logger = logger;
            _metrics = metrics;
            _sessionService = sessionService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> New([FromHeader(Name = "Authorization")][Required][StartsWith("Basic ")] string authorization)
        {
            try
            {
                var (login, password) = GetBasicAuthData(authorization);
                var (result, response) = await _sessionService.Login(new LoginRequest
                {
                    Login = login,
                    Password = password,
                    Location = this.GetRoughLocation(),
                    IpAddress = this.GetIpAddress(),
                    UserAgent = this.GetUserAgent(),
                });
                if (!result.Succeeded)
                {
                    var problem = ProcessErrorResult(result);
                    _metrics.SessionController().MarkBadLoginAttempt(problem);
                    return Unauthorized(new ProblemDetails
                    {
                        Title = problem
                    });
                }

                _metrics.SessionController().MarkSuccessfulLoginAttempt();
                return Ok(response);
            }
            catch (BadBasicAuthorizationDataException e)
            {
                _metrics.SessionController().MarkBadAuthData();
                _logger.LogInformation(e.Message, e);
                return BadRequest(new ProblemDetails
                {
                    Title = "authorization data is invalid"
                });
            }
            catch (Exception e)
            {
                _metrics.SessionController().MarkLoginException(e.GetType().FullName);
                _logger.LogError(e.Message, e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
                });
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(RefreshTokenSuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh(
            [FromHeader(Name = "Authorization")]
            [Required]
            [StartsWith("Bearer")]
            string authorization
        )
        {
            var token = GetBearerToken(authorization);
            try
            {
                var jwt = await _sessionService.Refresh(token);
                return Ok(new RefreshTokenSuccessResponse
                {
                    Jwt = jwt
                });
            }
            catch (RefreshTokenNotFoundException)
            {
                _metrics.SessionController().MarkTokenNotFoundInDb();
                return NotFound(new ProblemDetails
                {
                    Title = "token is invalid"
                });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "error processing request");
                _metrics.SessionController().MarkException(e.GetType().FullName);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Detail = "error processing request"
                });
            }
        }

        private static string ProcessErrorResult(Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (result.IsLockedOut)
            {
                return "Account Locked Out";
            }

            return result.IsNotAllowed ? "Login not allowed" : "Wrong Credentials";
        }

        private static string GetBearerToken(string authorizationHeader)
        {
            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        private static (string, string) GetBasicAuthData(string authorizationHeader)
        {
            try
            {
                var token = authorizationHeader.Substring("Basic ".Length).Trim();
                var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(":");
                return (parts[0], parts[1]);
            }
            catch (Exception e)
            {
                throw new BadBasicAuthorizationDataException("bad data", e);
            }
        }
    }
}
