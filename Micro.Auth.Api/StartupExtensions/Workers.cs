using Micro.Auth.Api.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Workers
    {
        public static void RegisterWorker(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }
    }
}
