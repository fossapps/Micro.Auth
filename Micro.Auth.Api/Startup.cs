using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL;
using Micro.Auth.Api.GraphQL.Inputs;
using Micro.Auth.Api.GraphQL.Types;
using Micro.Auth.Api.Internal.Configs;
using Micro.Auth.Api.Internal.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Micro.Auth.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureGraphQl();
            services.EnableFederation();
            services.AddConfiguration(Configuration);
            services.AddMetrics();
            services.ConfigureRequiredDependencies(Configuration);
            services.ConfigureHealthChecks();
            services.ConfigureIdentityServices(Configuration);
            services.AddControllers();
            services.ConfigureSwagger();
            services.RegisterWorkers();
            services.AddCorsPolicies(Configuration.GetSection("CorsConfig").Get<CorsConfig>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptions<CorsConfig> corsConfig)
        {
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCorsPolicy(corsConfig.Value);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.AddSwaggerWithUi();
            app.UseGraphQL<ISchema>();
            app.UseGraphQLGraphiQL(new GraphiQLOptions
            {
                SubscriptionsEndPoint = null,
                GraphQLEndPoint = "/graphql"
            }, "/ui/graphql");
            app.UseGraphQLPlayground(new PlaygroundOptions
            {
                GraphQLEndPoint = "/graphql",
            }, "/ui/playground");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.ConfigureHealthCheckEndpoint();
            });
        }
    }
}
