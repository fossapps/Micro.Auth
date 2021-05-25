using System.Linq;
using Micro.Auth.Api.Internal.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class Cors
    {
        public static void AddCorsPolicies(this IServiceCollection services, CorsConfig config)
        {
            services.AddCors(x =>
            {
                x.AddPolicy("development", builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                x.AddPolicy("production", builder =>
                {
                    builder.WithOrigins(config.Origins.ToArray())
                        .WithMethods(config.Headers.ToArray())
                        .AllowAnyMethod();
                    if (!config.AllowCredentials)
                    {
                        builder.DisallowCredentials();
                        return;
                    }
                    builder.AllowCredentials();
                });
            });
        }

        public static void UseCorsPolicy(this IApplicationBuilder app, CorsConfig config)
        {
            app.UseCors(config.PolicyToUse);
        }
    }
}
