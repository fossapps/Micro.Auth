using System.Threading.Tasks;
using Micro.Auth.Api.Controllers.ViewModels;
using Micro.Auth.Api.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UsersController(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        [HttpGet("findByEmail/{email}")]
        [ProducesResponseType(typeof(FindByEmailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindByEmail(string email)
        {
            var user = await _userRepository.FindByEmail(email);
            return Ok(new FindByEmailResponse
            {
                Available = user == null,
                Email = email,
            });
        }

        [HttpGet("findByUsername/{username}")]
        [ProducesResponseType(typeof(FindByUsernameResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindByUsername(string username)
        {
            var user = await _userRepository.FindByUsername(username);
            return Ok(new FindByUsernameResponse
            {
                Available = user == null,
                Username = username,
            });
        }


        [HttpPost]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var result = await _userService.Create(request);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
