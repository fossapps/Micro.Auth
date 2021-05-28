using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Micro.Auth.Business.Internal.Configs;
using Micro.Auth.Business.Internal.Keys;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Auth.Business.Internal.Tokens
{
    public interface ITokenFactory
    {
        string GenerateJwtToken(ClaimsPrincipal principal);
    }

    public class TokenFactory : ITokenFactory
    {
        private readonly IKeyContainer _keyContainer;
        private readonly IdentityOptions _identityOptions;
        private readonly IdentityConfig _identityConfig;

        public TokenFactory(IKeyContainer keyContainer, IOptions<IdentityOptions> identityOptions, IOptions<IdentityConfig> identityConfig)
        {
            _keyContainer = keyContainer;
            _identityOptions = identityOptions.Value;
            _identityConfig = identityConfig.Value;
        }

        public string GenerateJwtToken(ClaimsPrincipal principal)
        {
            var securityStampClaimType = _identityOptions.ClaimsIdentity.SecurityStampClaimType;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _identityConfig.Issuer,
                Audience = _identityConfig.IssueForAudience,
                Subject = new ClaimsIdentity(principal.Claims.Where(x => x.Type != securityStampClaimType)),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_keyContainer.GetKey().PrivateKey) {KeyId = _keyContainer.GetKey().KeyId}, SecurityAlgorithms.RsaSha512)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
