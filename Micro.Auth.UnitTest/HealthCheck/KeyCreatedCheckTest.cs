using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Auth.Api.Internal.HealthCheck;
using Micro.Auth.Business.Internal.Keys;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.HealthCheck
{
    public class KeyCreatedCheckTest
    {
        [Test]
        public async Task TestHealthCheckIsUnhealthyIfNoKeyExists()
        {
            var keyContainer = new Mock<IKeyContainer>();
            keyContainer.Setup(x => x.GetKey()).Returns(null as SigningKey);
            var keyCreatedHealthCheck = new KeyCreatedCheck(keyContainer.Object);
            var response = await keyCreatedHealthCheck.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }

        [Test]
        public async Task TestHealthCheckIsUnhealthyIfKeyWasCreatedMoreThan10MinutesAnd30SecondsAgo()
        {
            var keyContainer = new Mock<IKeyContainer>();
            keyContainer.Setup(x => x.GetKey()).Returns(new SigningKey
            {
                CreatedAt = DateTime.Now - TimeSpan.FromMinutes(10) - TimeSpan.FromSeconds(31)
            });
            var keyCreatedHealthCheck = new KeyCreatedCheck(keyContainer.Object);
            var response = await keyCreatedHealthCheck.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }

        [Test]
        public async Task TestHealthCheckIsDegradedIfKeyWasCreatedMoreThan5MinutesAnd30SecondsAgo()
        {
            var keyContainer = new Mock<IKeyContainer>();
            keyContainer.Setup(x => x.GetKey()).Returns(new SigningKey
            {
                CreatedAt = DateTime.Now - TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(31)
            });
            var keyCreatedHealthCheck = new KeyCreatedCheck(keyContainer.Object);
            var response = await keyCreatedHealthCheck.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Degraded, response.Status);
        }

        [Test]
        public async Task TestHealthCheckIsHealthyIfKeyCreatedIsRecent()
        {
            var keyContainer = new Mock<IKeyContainer>();
            keyContainer.Setup(x => x.GetKey()).Returns(new SigningKey
            {
                CreatedAt = DateTime.Now - TimeSpan.FromMinutes(1)
            });
            var keyCreatedHealthCheck = new KeyCreatedCheck(keyContainer.Object);
            var response = await keyCreatedHealthCheck.CheckHealthAsync(null, CancellationToken.None);
            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }
    }
}
