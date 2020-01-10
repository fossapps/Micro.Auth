using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Micro.Auth.Api.Keys;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Auth.Api.Tokens
{
    public interface ITokenFactory
    {
        string GenerateJwtToken(ClaimsPrincipal principal);
    }

    public class TokenFactory : ITokenFactory
    {

        private readonly IKeyContainer _keyContainer;
        private readonly IdentityOptions _identityOptions;

        public TokenFactory(IKeyContainer keyContainer, IOptions<IdentityOptions> identityOptions)
        {
            _keyContainer = keyContainer;
            _identityOptions = identityOptions.Value;
        }

        public string GenerateJwtToken(ClaimsPrincipal principal)
        {
            var securityStampClaimType = _identityOptions.ClaimsIdentity.SecurityStampClaimType;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "micro.auth",
                Audience = "micro.auth",
                Subject = new ClaimsIdentity(principal.Claims.Where(x => x.Type != securityStampClaimType)),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_keyContainer.GetKey().PrivateKey) {KeyId = _keyContainer.GetKey().KeyId}, SecurityAlgorithms.RsaSha512)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
