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
            services.AddDbContext<ApplicationContext>(ServiceLifetime.Transient);
            services.AddTransient<DbContext, ApplicationContext>();
            services.AddSingleton<IUuidService, UuidService>();
            services.AddSingleton<IKeyContainer, KeyContainer>();
            services.AddTransient<IRoleStore<IdentityRole>, RoleStore<IdentityRole>>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPasswordManager, PasswordManager>();
            services.AddTransient<IEmailVerificationService, EmailVerificationService>();
            services.AddTransient<IAvailabilityService, AvailabilityService>();
            services.AddSingleton<IKeyResolver, KeyResolver>();
            services.AddSingleton<ITokenFactory, TokenFactory>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<ISessionService, SessionService>();
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
