using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Tokens;
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
            var key = SigningKey.Create();
            key.KeyId = "keyId";
            keyContainer.Setup(x => x.GetKey()).Returns(key);
            var tokenFactory = new TokenFactory(keyContainer.Object);
            var claims = new []
            {
                new Claim("name", "Alice"),
                new Claim("role", "sudo"),
            };
            var claimsPrincipal = new Mock<ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Claims).Returns(claims);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenFactory.GenerateJwtToken(claimsPrincipal.Object));
            Assert.AreEqual("keyId", token.Header.Kid);
            Assert.AreEqual("Alice", token.Claims.FirstOrDefault(x => x.Type == "name")?.Value);
            Assert.AreEqual("sudo", token.Claims.FirstOrDefault(x => x.Type == "role")?.Value);
        }
    }
}
