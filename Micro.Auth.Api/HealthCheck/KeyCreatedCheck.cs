using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Auth.Business.Keys;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Micro.Auth.Api.HealthCheck
{
    public class KeyCreatedCheck : IHealthCheck
    {
        private readonly IKeyContainer _keyContainer;
        public KeyCreatedCheck(IKeyContainer keyContainer)
        {
            _keyContainer = keyContainer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var key = _keyContainer.GetKey();
            if (key == null)
            {
                return HealthCheckResult.Unhealthy("Key isn't generated or doesn't exist");
            }

            if (key.CreatedAt + TimeSpan.FromMinutes(10) + TimeSpan.FromSeconds(30) < DateTime.Now)
            {
                return HealthCheckResult.Unhealthy("key wasn't generated in last 10 minutes 30 seconds");
            }

            var elapsedSinceCreated = DateTime.Now - key.CreatedAt;
            return key.CreatedAt + TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(30) < DateTime.Now
                ? HealthCheckResult.Degraded($"key was generated {elapsedSinceCreated} ago")
                : HealthCheckResult.Healthy($"key was generated {elapsedSinceCreated} ago");
        }
    }
}
