using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Micro.Auth.Api.Configs;
using Micro.Auth.Business.Configs;
using Micro.Auth.Business.Keys;
using Micro.Auth.Business.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Tokens
{
    public class TokenFactoryTest
    {
        [Test]
        public void TestCreateJwt()
        {
            var keyContainer = new Mock<IKeyContainer>();
            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            mockOptions.Setup(x => x.Value).Returns(new IdentityOptions
            {
                ClaimsIdentity = new ClaimsIdentityOptions
                {
                    SecurityStampClaimType = "security"
                }
            });
            var key = SigningKey.Create();
            key.KeyId = "keyId";
            keyContainer.Setup(x => x.GetKey()).Returns(key);
            var mockIdentityConfigOption = new Mock<IOptions<IdentityConfig>>();
            mockIdentityConfigOption.Setup(x => x.Value).Returns(new IdentityConfig
            {
                Audiences = new List<string> {"aud1", "aud2"},
                Issuer = "auth_test",
                IssueForAudience = "issuer"
            });
            var tokenFactory = new TokenFactory(keyContainer.Object, mockOptions.Object, mockIdentityConfigOption.Object);
            var claims = new []
            {
                new Claim("name", "Alice"),
                new Claim("role", "sudo"),
                new Claim("security", "secure_data"),
            };
            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Claims).Returns(claims);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenFactory.GenerateJwtToken(claimsPrincipal.Object));
            Assert.AreEqual("keyId", token.Header.Kid);
            Assert.AreEqual("Alice", token.Claims.FirstOrDefault(x => x.Type == "name")?.Value);
            Assert.AreEqual("sudo", token.Claims.FirstOrDefault(x => x.Type == "role")?.Value);
            Assert.IsNull(token.Claims.FirstOrDefault(x => x.Type == "security")?.Value);
        }
    }
}
