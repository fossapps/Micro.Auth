using Micro.Auth.Business.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Workers
    {
        public static void RegisterWorkers(this IServiceCollection services)
        {
            services.AddHostedService<KeyGenerationWorker>();
        }
    }
}
