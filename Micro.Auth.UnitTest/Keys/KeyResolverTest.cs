using System.Threading;
using System.Threading.Tasks;
using FossApps.KeyStore;
using FossApps.KeyStore.Models;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Keys.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Rest;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Keys
{
    public class KeyResolverTest
    {
        [Test]
        public async Task TestKeyResolverReturnsKey()
        {
            var mockKeyStoreClient = new Mock<IKeyStoreClient>();
            mockKeyStoreClient.Setup(x => x.Keys.GetWithHttpMessagesAsync("keyId", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<object>
                {
                    Body = new KeyCreatedResponse("testBody", "keyId")
                });

            var keyResolver = new KeyResolver(mockKeyStoreClient.Object);
            var key = await keyResolver.ResolveKey("keyId");
            Assert.AreEqual("testBody", key);
        }

        [Test]
        public void TestKeyResolverThrowsWhenKeyIsNotFound()
        {
            var mockKeyStoreClient = new Mock<IKeyStoreClient>();
            mockKeyStoreClient.Setup(x => x.Keys.GetWithHttpMessagesAsync("keyId", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<object>
                {
                    Body = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound
                    }
                });

            var keyResolver = new KeyResolver(mockKeyStoreClient.Object);
            Assert.ThrowsAsync<KeyNotFoundException>(() => keyResolver.ResolveKey("keyId"), "key: 'keyId' not found");
        }
    }
}
