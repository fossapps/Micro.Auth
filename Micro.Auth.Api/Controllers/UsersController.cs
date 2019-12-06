using System.Threading.Tasks;
using Micro.Auth.Api.Controllers.ViewModels;
using Micro.Auth.Api.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
