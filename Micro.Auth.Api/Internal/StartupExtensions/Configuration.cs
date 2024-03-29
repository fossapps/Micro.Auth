using Micro.Auth.Api.Internal.Configs;
using Micro.Auth.Business.Internal.Configs;
using Micro.Auth.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class Configuration
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));
            services.Configure<Mail>(configuration.GetSection("EmailConfig"));
            services.Configure<ElasticConfiguration>(configuration.GetSection("ElasticConfiguration"));
            services.Configure<CorsConfig>(configuration.GetSection("CorsConfig"));
            services.Configure<KeyGenerationConfig>(configuration.GetSection("KeyGenerationConfig"));
            services.Configure<IdentityConfig>(configuration.GetSection("IdentityConfig"));
        }
    }
}
