using Micro.Auth.Api.Keys;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyController : ControllerBase
    {
        private readonly IKeyContainer _keyContainer;

        public KeyController(IKeyContainer keyContainer)
        {
            _keyContainer = keyContainer;
        }

        [HttpGet]
        public IActionResult GetKey()
        {
            return Ok(new {_keyContainer.GetKey().PublicKey, _keyContainer.GetKey().KeyId});
        }
    }
}
