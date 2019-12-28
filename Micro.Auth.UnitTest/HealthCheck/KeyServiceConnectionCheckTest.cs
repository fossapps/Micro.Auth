using System;
using System.Threading;
using System.Threading.Tasks;
using FossApps.KeyStore;
using FossApps.KeyStore.Models;
using Micro.Auth.Api.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.HealthCheck
{
    public class KeyServiceConnectionCheckTest
    {
        [Test]
        public async Task TestHealthCheckIsUnhealthyIfCannotConnect()
        {
            var mockClient = new Mock<IKeyStoreClient>();
            mockClient.Setup(x => x.BasicPing.PingWithHttpMessagesAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as HttpOperationResponse<PingResponse>);
            var check = new KeyServiceConnectionCheck(mockClient.Object, null);
            var result = await check.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }

        [Test]
        public async Task TestHealthCheckIsHealthyIfCanConnect()
        {
            var mockClient = new Mock<IKeyStoreClient>();
            mockClient.Setup(x => x.BasicPing.PingWithHttpMessagesAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpOperationResponse<PingResponse>());
            var check = new KeyServiceConnectionCheck(mockClient.Object, null);
            var result = await check.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }

        [Test]
        public async Task TestHealthCheckIsUnhealthyIfExceptionIsThrown()
        {
            var mockClient = new Mock<IKeyStoreClient>();
            mockClient.Setup(x => x.BasicPing.PingWithHttpMessagesAsync(null, It.IsAny<CancellationToken>()))
                .Throws<Exception>();
            var mockLogger = new Mock<ILogger<KeyServiceConnectionCheck>>();
            var check = new KeyServiceConnectionCheck(mockClient.Object, mockLogger.Object);
            var result = await check.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }
    }
}
