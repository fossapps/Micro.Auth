using System;
using System.Threading.Tasks;
using App.Metrics;
using Micro.Auth.Api.Measurements;
using Micro.Auth.Api.Users.ViewModels;
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

        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            try
            {
                var result = await _metrics.UsersControllerMetrics().RecordTimeCreateUser(async () => await _userService.Create(request));
                if (!result.Succeeded)
                {
                    _logger.LogDebug("bad request: ", result.Errors);
                    _metrics.UsersControllerMetrics().MarkBadRequest();
                    return BadRequest(result);
                }

                _metrics.UsersControllerMetrics().MarkAccountCreated();
                return Ok(result);
            }
            catch (Exception e)
            {
                _metrics.UsersControllerMetrics().MarkCreateAccountException(e.GetType().FullName);
                _logger.LogCritical(e, "error while trying to create user");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "error handling request",
                });
            }
        }
    }
}
