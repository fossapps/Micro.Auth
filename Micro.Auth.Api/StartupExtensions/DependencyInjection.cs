using Micro.Auth.Api.Models;
using Micro.Auth.Api.Repository;
using Micro.Auth.Api.Uuid;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddSingleton<IUuidService, UuidService>();
        }
    }
}