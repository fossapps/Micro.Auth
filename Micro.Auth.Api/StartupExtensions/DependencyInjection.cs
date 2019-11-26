using System;
using FossApps.KeyStore;
using Micro.Auth.Api.Configs;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Models;
using Micro.Auth.Api.Uuid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddSingleton<IUuidService, UuidService>();
            services.AddSingleton<IKeyContainer, KeyContainer>();
            services.AddSingleton(SetupKeyStoreHttpClient(configuration.GetSection("Services").Get<Services>().KeyStore));
        }

        private static IKeyStoreClient SetupKeyStoreHttpClient(KeyStoreConfig config)
        {
            return new KeyStoreClient
            {
                BaseUri = new Uri(config.Url),
            };
        }
    }
}
