using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Micro.Auth.Api.GraphQL;
using Micro.Auth.Api.GraphQL.DataLoaders;
using Micro.Auth.Api.GraphQL.Directives;
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
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<IDocumentExecutionListener, DataLoaderDocumentListener>();
            services.AddTransient<Query>();
            services.AddTransient<Mutation>();
            services.AddTransient<UserType>();
            services.AddTransient<AvailabilityResultType>();
            services.AddTransient<ResultType>();
            services.AddTransient<RegisterInputType>();
            services.AddTransient<RefreshTokenType>();
            services.AddTransient<ChangePasswordInput>();
            services.AddTransient<ResetPasswordInput>();
            services.AddTransient<VerifyEmailInputType>();
            services.AddTransient<ISchema, AuthSchema>();
            services.AddTransient<UserByIdDataLoader>();
            services.AddTransient<SessionByUserDataLoader>();
            services.AddScoped<AuthorizeDirectiveVisitor>();
            services
                .AddGraphQL(options =>
                {
                    options.UnhandledExceptionDelegate = ctx =>
                    {
                        ctx.ErrorMessage = ctx.OriginalException.Message;
                    };
                })
                .AddDataLoader()
                .AddSystemTextJson()
                .AddErrorInfoProvider(opts => opts.ExposeExceptionStackTrace = false);
        }
    }
}