using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Micro.Auth.Api.Keys;
using Micro.Auth.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PemUtils;

namespace Micro.Auth.Api.StartupExtensions
{
    public static class Identity
    {
        public static void ConfigureIdentityServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddIdentity<User, IdentityRole>(options =>
                {
                    options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
                })
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            serviceCollection.AddAuthorization();
            serviceCollection.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
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
                        ValidIssuer = "micro.auth",
                        ValidAudiences = new []{"micro.auth", "micro.cart"},
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                            {
                                // todo: I know this .Result is a very bad idea (converting from async to sync)
                                // however there's no other way to do this, signing key resolver doesn't have a
                                // async version of this method, they are looking into it though
                                // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/468
                                var key = serviceCollection.BuildServiceProvider().GetRequiredService<IKeyResolver>()
                                    .ResolveKey(kid).Result;
                                var pemReader = new PemReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(key)));
                                var publicKeyParameters = pemReader.ReadRsaKey();
                                return new []{new RsaSecurityKey(publicKeyParameters)};
                            }
                    };
                });
            serviceCollection.Configure<IdentityOptions>(ConfigureIdentityOptions);
        }

        private static void ConfigureIdentityOptions(IdentityOptions options)
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 3;

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
        }
    }
}
