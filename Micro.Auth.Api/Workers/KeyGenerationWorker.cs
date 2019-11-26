using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using FossApps.KeyStore;
using FossApps.KeyStore.Models;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Measurements;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Workers
{
    // ReSharper disable once ClassNeverInstantiated.Global This is Worker and will be initialized by framework.
    public class KeyGenerationWorker : BackgroundService
    {
        private readonly ILogger<KeyGenerationWorker> _logger;
        private readonly IKeyContainer _keyContainer;
        private readonly IKeyStoreClient _keyStoreClient;
        private readonly IMetrics _metrics;

        public KeyGenerationWorker(ILogger<KeyGenerationWorker> logger, IKeyContainer keyContainer, IKeyStoreClient keyStoreClient, IMetrics metrics)
        {
            _logger = logger;
            _keyContainer = keyContainer;
            _keyStoreClient = keyStoreClient;
            _metrics = metrics;
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

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task<object> SaveKey(string publicKey, CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _keyStoreClient.Keys.AddAsync(new CreateKeyRequest
            {
                Body = publicKey
            }, stoppingToken);
            _logger.LogInformation(stopwatch.ElapsedMilliseconds.ToString());
            return response;
        }
    }
}
