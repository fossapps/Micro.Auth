using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Micro.Auth.Api.Keys;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Auth.Api.Tokens
{
    public interface ITokenFactory
    {
        string GenerateToken(int size);
        string GenerateJwtToken(ClaimsPrincipal principal);
    }

    public class TokenFactory : ITokenFactory
    {

        private readonly IKeyContainer _keyContainer;

        public TokenFactory(IKeyContainer keyContainer)
        {
            _keyContainer = keyContainer;
        }

        public string GenerateToken(int size)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateJwtToken(ClaimsPrincipal principal)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "micro.auth",
                Audience = "micro.auth",
                Subject = new ClaimsIdentity(principal.Claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_keyContainer.GetKey().PrivateKey) {KeyId = _keyContainer.GetKey().KeyId}, SecurityAlgorithms.RsaSha512)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}