using System;
using Fossapps.Micro.KeyStore;
using Micro.Auth.Api.Configs;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.RefreshTokens;
using Micro.Auth.Api.Tokens;
using Micro.Auth.Api.Users;
using Micro.Auth.Common;
using Micro.Auth.Storage;
using Micro.Mails;
using Micro.Mails.Content;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddScoped<DbContext, ApplicationContext>();
            services.AddSingleton<IUuidService, UuidService>();
            services.AddSingleton<IKeyContainer, KeyContainer>();
            services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IKeyResolver, KeyResolver>();
            services.AddSingleton<ITokenFactory, TokenFactory>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.SetupMail(configuration);
            services.AddSingleton(SetupKeyStoreHttpClient(configuration.GetSection("Services").Get<Services>().KeyStore));
        }

        private static void SetupMail(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailConfig").Get<Mail>();
            services.AddSingleton(new EmailUrlBuilder(emailConfig.EmailUrlConfig));
            services.AddSingleton<IMailService>(new SmtpMailService(emailConfig.Smtp));
            services.AddSingleton(new MailBuilder(emailConfig.DefaultSender));
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
