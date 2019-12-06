using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Micro.Auth.Api.Keys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IActionResult> GetKey()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "micro.auth",
                Audience = "micro.auth",
                Subject = new ClaimsIdentity(new []{new Claim("permission", "sudo"), }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_keyContainer.GetKey().PrivateKey) {KeyId = _keyContainer.GetKey().KeyId}, SecurityAlgorithms.RsaSha512)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var result = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return Ok(result);
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestKey()
        {
            return Ok("looks good");
        }
    }
}
