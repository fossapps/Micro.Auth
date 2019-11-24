using System;
using System.Threading;
using System.Threading.Tasks;
using FossApps.KeyStore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.HealthCheck
{
    public class KeyServiceConnectionCheck : IHealthCheck
    {
        private readonly IHttpClient _client;
        private readonly ILogger<KeyServiceConnectionCheck> _logger;

        public KeyServiceConnectionCheck(IHttpClient client, ILogger<KeyServiceConnectionCheck> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var result = await _client.BasicPing.PingWithHttpMessagesAsync(null, cancellationToken);
                if (result != null)
                {
                    return HealthCheckResult.Healthy("can ping");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "caught exception");
            }
            return HealthCheckResult.Unhealthy("can't ping keyservice");
        }
    }
}
