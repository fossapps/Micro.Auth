using System.Linq;
using Micro.Auth.Api.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Micro.Auth.Api.StartupExtensions
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
                            .AllowAnyMethod()
                            .AllowCredentials();
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
