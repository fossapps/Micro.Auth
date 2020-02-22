using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using App.Metrics;
using Micro.Auth.Api.Authentication.Exceptions;
using Micro.Auth.Api.Measurements;
using Micro.Auth.Api.UserData.Extensions;
using Micro.Auth.Api.Users;
using Micro.Auth.Api.Users.ViewModels;
using Micro.Auth.Api.ValidationAttributes;
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
        private readonly IUserService _userService;
        private readonly IMetrics _metrics;

        public SessionController(ILogger<SessionController> logger, IUserService userService, IMetrics metrics)
        {
            _logger = logger;
            _userService = userService;
            _metrics = metrics;
        }

        [HttpPost("new")]
        public async Task<IActionResult> New([FromHeader(Name = "Authorization")][Required][StartsWith("Basic ")] string authorization)
        {
            try
            {
                var (login, password) = GetBasicAuthData(authorization);
                var (result, response) = await _userService.Login(new LoginRequest
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
                _logger.LogInformation(e.Message, e);
                return BadRequest(new ProblemDetails
                {
                    Title = "authorization data is invalid"
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message, e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "error handling request"
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
