using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class Swagger
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescription => apiDescription.Last());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Micro.Auth",
                    Version = "v1"
                });
            });
        }

        public static void AddSwaggerWithUi(this IApplicationBuilder app)
        {
            app.UseSwagger(x => x.SerializeAsV2 = true);
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "";
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });
        }
    }
}