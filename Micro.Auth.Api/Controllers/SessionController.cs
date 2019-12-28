using System;
using System.Text;
using System.Threading.Tasks;
using Micro.Auth.Api.Controllers.Extensions;
using Micro.Auth.Api.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IUserService _userService;

        public SessionController(ILogger<SessionController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> New([FromHeader(Name = "Authorization")] string authorization)
        {
            if (authorization == null)
            {
                return BadRequest();
            }


            if (!authorization.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only basic authentication supported for creating new sessions");
            }
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
                return Unauthorized(ProcessErrorResult(result));
            }
            return Ok(response);
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
            var token = authorizationHeader.Substring("Basic ".Length).Trim();
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(":");
            return (parts[0], parts[1]);
        }
    }
}
