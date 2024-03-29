using Micro.Auth.Business.Internal.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class Workers
    {
        public static void RegisterWorkers(this IServiceCollection services)
        {
            services.AddHostedService<KeyGenerationWorker>();
        }
    }
}
