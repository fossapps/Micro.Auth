using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Auth.Api.Keys;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IKeyContainer _keyContainer;

        public Worker(ILogger<Worker> logger, IKeyContainer keyContainer)
        {
            _logger = logger;
            _keyContainer = keyContainer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at {DateTime.Now}");
                _keyContainer.SetKey(SigningKey.Create());
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
