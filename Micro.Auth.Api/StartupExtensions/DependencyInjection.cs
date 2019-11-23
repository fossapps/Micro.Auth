using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Models;
using Micro.Auth.Api.Uuid;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddSingleton<IUuidService, UuidService>();
            services.AddSingleton<IKeyContainer, KeyContainer>();
        }
    }
}
