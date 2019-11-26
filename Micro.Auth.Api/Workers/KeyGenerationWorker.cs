using System;
using System.Threading;
using System.Threading.Tasks;
using FossApps.KeyStore;
using FossApps.KeyStore.Models;
using Micro.Auth.Api.Keys;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Workers
{
    public class KeyGenerationWorker : BackgroundService
    {
        private readonly ILogger<KeyGenerationWorker> _logger;
        private readonly IKeyContainer _keyContainer;
        private readonly IKeyStoreClient _keyStoreClient;

        public KeyGenerationWorker(ILogger<KeyGenerationWorker> logger, IKeyContainer keyContainer, IKeyStoreClient keyStoreClient)
        {
            _logger = logger;
            _keyContainer = keyContainer;
            _keyStoreClient = keyStoreClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at {DateTime.Now}");
                var key = SigningKey.Create();
                var response = await _keyStoreClient.Keys.AddAsync(new CreateKeyRequest
                {
                    Body = key.PublicKey
                }, stoppingToken);
                switch (response)
                {
                    case KeyCreatedResponse createdResponse:
                        key.KeyId = createdResponse.Id;
                        _keyContainer.SetKey(key);
                        break;
                    case ProblemDetails _:
                        _logger.LogError("problem while saving public key");
                        break;
                    default:
                        _logger.LogError("unhandled type");
                        break;
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
