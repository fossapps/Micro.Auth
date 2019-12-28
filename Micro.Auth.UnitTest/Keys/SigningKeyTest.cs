using Micro.Auth.Api.Keys;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Keys
{
    public class SigningKeyTest
    {
        [Test]
        public void TestCreateReturnsPrivateAndPublicKey()
        {
            var result = SigningKey.Create();
            Assert.NotNull(result.PrivateKey);
            Assert.NotNull(result.PublicKey);
            Assert.NotNull(result.CreatedAt);
        }

        [Test]
        public void TestPublicKeyIsStoredInPemFormat()
        {
            var result = SigningKey.Create();
            Assert.IsTrue(result.PublicKey.StartsWith("-----BEGIN PUBLIC KEY-----\n"));
            Assert.IsTrue(result.PublicKey.EndsWith("-----END PUBLIC KEY-----\n"));
        }
    }
}
