using System;
using Fossapps.Micro.KeyStore;
using Micro.Auth.Api.Internal.Configs;
using Micro.Auth.Business.Availability;
using Micro.Auth.Business.EmailVerification;
using Micro.Auth.Business.Internal.Keys;
using Micro.Auth.Business.Internal.Tokens;
using Micro.Auth.Business.PasswordManager;
using Micro.Auth.Business.Sessions;
using Micro.Auth.Business.Users;
using Micro.Auth.Common;
using Micro.Auth.Storage;
using Micro.Mails;
using Micro.Mails.Content;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Auth.Api.Internal.StartupExtensions
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
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddSingleton<IKeyResolver, KeyResolver>();
            services.AddSingleton<ITokenFactory, TokenFactory>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ISessionService, SessionService>();
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
