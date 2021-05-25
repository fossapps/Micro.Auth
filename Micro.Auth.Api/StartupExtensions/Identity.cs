using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using Micro.Auth.Api.Configs;
using Micro.Auth.Business.Configs;
using Micro.Auth.Business.Keys;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PemUtils;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Identity
    {
        public static void ConfigureIdentityServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddIdentity<User, IdentityRole>(options =>
                {
                    options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            serviceCollection.AddAuthorization();
            var identityConfig = configuration.GetSection("IdentityConfig").Get<IdentityConfig>();
            serviceCollection.ConfigureAuthentication(identityConfig);
            serviceCollection.Configure<IdentityOptions>(x =>
            {
                ConfigureIdentityOptions(x, configuration.GetSection("IdentityConfig").Get<IdentityConfig>());
            });
        }

        private static void ConfigureAuthentication(this IServiceCollection services, IdentityConfig identityConfig)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                ConfigureJwtBearer(services, config, identityConfig);
            });
        }

        private static void ConfigureJwtBearer(IServiceCollection services, JwtBearerOptions config, IdentityConfig identityConfig)
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = false;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateActor = true,
                ValidateAudience = true,
                ValidIssuer = identityConfig.Issuer,
                ValidAudiences = identityConfig.Audiences.ToArray(),
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // todo: I know this .Result is a very bad idea (converting from async to sync)
                    // however there's no other way to do this, signing key resolver doesn't have a
                    // async version of this method, they are looking into it though
                    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/468
                    var key = services.BuildServiceProvider().GetRequiredService<IKeyResolver>()
                        .ResolveKey(kid).Result;
                    var pemReader = new PemReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(key)));
                    var publicKeyParameters = pemReader.ReadRsaKey();
                    return new []{new RsaSecurityKey(publicKeyParameters)};
                }
            };
        }

        private static void ConfigureIdentityOptions(IdentityOptions options, IdentityConfig config)
        {
            options.Password = config.PasswordRequirements;
            options.Lockout = config.LockoutOptions.ToIdentityLockoutOptions();
            options.User = config.UserOptions;
            options.SignIn = config.SignInOptions;
        }
    }
}
