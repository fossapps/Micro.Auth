using System;
using System.IO;
using Fossapps.Micro.KeyStore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PemUtils;

namespace Micro.Auth.Sdk
{
    public static class ConfigureIdentity
    {
        public static void ConfigureAuthServices(this IServiceCollection services, Config config)
        {
            services.AddSingleton<IKeyResolver, KeyResolver>();
            services.AddSingleton(SetupKeyStoreClient(config.KeyStoreUrl));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                ConfigureJwtBearer(services, options, config);
            });
        }

        private static void ConfigureJwtBearer(IServiceCollection services, JwtBearerOptions options, Config config)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = config.ValidateIssuer,
                ValidateIssuerSigningKey = config.ValidateIssuerSigningKey,
                ValidateLifetime = config.ValidateLifetime,
                ValidateActor = config.ValidateActor,
                ValidateAudience = config.ValidateAudience,
                ValidIssuer = config.ValidIssuer,
                ValidAudiences = config.ValidAudiences,
                ClockSkew = config.ClockSkew,
                IssuerSigningKeyResolver = (token, secToken, kid, parameters) =>
                {
                    var key = services.BuildServiceProvider().GetRequiredService<IKeyResolver>()
                        .ResolveKey(kid).Result;
                    var pemReader = new PemReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(key)));
                    var publicKeyParameters = pemReader.ReadRsaKey();
                    return new []{new RsaSecurityKey(publicKeyParameters)};                }
            };
        }

        private static IKeyStoreClient SetupKeyStoreClient(string keystoreUrl)
        {
            return new KeyStoreClient
            {
                BaseUri = new Uri(keystoreUrl)
            };
        }

        public static void SetupAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
