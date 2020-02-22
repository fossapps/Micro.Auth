using Micro.Auth.Api.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Configuration
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));
            services.Configure<Mail>(configuration.GetSection("EmailConfig"));
            services.Configure<ElasticConfiguration>(configuration.GetSection("ElasticConfiguration"));
            services.Configure<CorsConfig>(configuration.GetSection("CorsConfig"));
        }
    }
}
