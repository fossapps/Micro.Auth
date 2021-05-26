using GraphQL;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL;
using Micro.Auth.Api.GraphQL.Inputs;
using Micro.Auth.Api.GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class GraphQl
    {
        public static void ConfigureGraphQl(this IServiceCollection services)
        {
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddScoped<Query>();
            services.AddScoped<Mutation>();
            services.AddScoped<UserType>();
            services.AddScoped<ResultType>();
            services.AddScoped<RegisterInputType>();
            services.AddScoped<VerifyEmailInputType>();
            services.AddScoped<ISchema, AuthSchema>();
            services
                .AddGraphQL(options =>
                {
                    options.UnhandledExceptionDelegate = ctx =>
                    {
                        ctx.ErrorMessage = ctx.OriginalException.Message;
                    };
                })
                .AddSystemTextJson()
                .AddErrorInfoProvider(opts => opts.ExposeExceptionStackTrace = false);
        }
    }
}