using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Fossapps.Micro.KeyStore;
using Fossapps.Micro.KeyStore.Models;
using Micro.Auth.Business.Internal.Configs;
using Micro.Auth.Business.Internal.Keys;
using Micro.Auth.Business.Internal.Measurements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.Auth.Business.Internal.Workers
{
    // ReSharper disable once ClassNeverInstantiated.Global This is Worker and will be initialized by framework.
    public class KeyGenerationWorker : BackgroundService
    {
        private readonly ILogger<KeyGenerationWorker> _logger;
        private readonly IKeyContainer _keyContainer;
        private readonly IKeyStoreClient _keyStoreClient;
        private readonly IMetrics _metrics;
        private readonly KeyGenerationConfig _keyGenerationConfig;

        public KeyGenerationWorker(ILogger<KeyGenerationWorker> logger, IKeyContainer keyContainer, IKeyStoreClient keyStoreClient, IMetrics metrics, IOptions<KeyGenerationConfig> keyGenerationConfig)
        {
            _logger = logger;
            _keyContainer = keyContainer;
            _keyStoreClient = keyStoreClient;
            _metrics = metrics;
            _keyGenerationConfig = keyGenerationConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at {DateTime.Now}");
                var key = SigningKey.Create();
                var response = await _metrics.KeyGenerationWorker().RecordTimeToSavePublicKey(async () => await SaveKey(key.PublicKey, stoppingToken ));
                switch (response)
                {
                    case KeyCreatedResponse createdResponse:
                        key.KeyId = createdResponse.Id;
                        _keyContainer.SetKey(key);
                        break;
                    case ProblemDetails _:
                        _metrics.KeyGenerationWorker().MarkErrorSavingToKeyService();
                        _logger.LogError("problem while saving public key");
                        break;
                    default:
                        _metrics.KeyGenerationWorker().MarkErrorUnhandledType();
                        _logger.LogError("unhandled type");
                        break;
                }

                await Task.Delay(TimeSpan.FromSeconds(_keyGenerationConfig.TimeBetweenGenerationInSeconds), stoppingToken);
            }
        }

        private async Task<object> SaveKey(string publicKey, CancellationToken stoppingToken)
        {
            return await _keyStoreClient.Keys.AddAsync(new CreateKeyRequest
            {
                Body = publicKey
            }, stoppingToken);
        }
    }
}
