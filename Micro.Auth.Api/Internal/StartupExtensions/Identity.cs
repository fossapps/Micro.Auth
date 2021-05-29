using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using Micro.Auth.Business.Internal.Configs;
using Micro.Auth.Business.Internal.Keys;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using PemUtils;

namespace Micro.Auth.Api.Internal.StartupExtensions
{
    public static class Identity
    {
        public static void ConfigureIdentityServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransientIdentity<User, IdentityRole>(options =>
                {
                    options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
                })
                .AddEntityFrameworkStoresTransient<ApplicationContext>()
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

        private static IdentityBuilder AddTransientIdentity<TUser, TRole>(
            this IServiceCollection services,
            Action<IdentityOptions>? setupAction = null)
            where TUser : class
            where TRole : class
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(IdentityConstants.ExternalScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.ExternalScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                };
            })
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
            {
                o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });
            services.AddHttpContextAccessor();
            // Identity services
            services.AddTransient<IUserValidator<TUser>, UserValidator<TUser>>();
            services.AddTransient<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.AddTransient<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.AddTransient<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddTransient<IRoleValidator<TRole>, RoleValidator<TRole>>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.AddTransient<IdentityErrorDescriber>();
            services.AddTransient<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.AddTransient<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
            services.AddTransient<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
            services.AddTransient<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();
            services.AddTransient<UserManager<TUser>>();
            services.AddTransient<SignInManager<TUser>>();
            services.AddTransient<RoleManager<TRole>>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }

        private static IdentityBuilder AddEntityFrameworkStoresTransient<TContext>(this IdentityBuilder builder)
        where TContext : DbContext
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType, typeof(TContext));
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type contextType)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null)
            {
                throw new InvalidOperationException("Not an identity user");
            }

            var keyType = identityUserType.GenericTypeArguments[0];

            if (roleType != null)
            {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
                if (identityRoleType == null)
                {
                    throw new InvalidOperationException("Not an identity role");
                }

                Type userStoreType = null;
                Type roleStoreType = null;
                var identityContext = FindGenericBaseType(contextType, typeof(IdentityDbContext<,,,,,,,>));
                if (identityContext == null)
                {
                    // If its a custom DbContext, we can only add the default POCOs
                    userStoreType = typeof(UserStore<,,,>).MakeGenericType(userType, roleType, contextType, keyType);
                    roleStoreType = typeof(RoleStore<,,>).MakeGenericType(roleType, contextType, keyType);
                }
                else
                {
                    userStoreType = typeof(UserStore<,,,,,,,,>).MakeGenericType(userType, roleType, contextType,
                        identityContext.GenericTypeArguments[2],
                        identityContext.GenericTypeArguments[3],
                        identityContext.GenericTypeArguments[4],
                        identityContext.GenericTypeArguments[5],
                        identityContext.GenericTypeArguments[7],
                        identityContext.GenericTypeArguments[6]);
                    roleStoreType = typeof(RoleStore<,,,,>).MakeGenericType(roleType, contextType,
                        identityContext.GenericTypeArguments[2],
                        identityContext.GenericTypeArguments[4],
                        identityContext.GenericTypeArguments[6]);
                }
                services.TryAddTransient(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
                services.TryAddTransient(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);
            }
            else
            {   // No Roles
                Type userStoreType = null;
                var identityContext = FindGenericBaseType(contextType, typeof(IdentityUserContext<,,,,>));
                if (identityContext == null)
                {
                    // If its a custom DbContext, we can only add the default POCOs
                    userStoreType = typeof(UserOnlyStore<,,>).MakeGenericType(userType, contextType, keyType);
                }
                else
                {
                    userStoreType = typeof(UserOnlyStore<,,,,,>).MakeGenericType(userType, contextType,
                        identityContext.GenericTypeArguments[1],
                        identityContext.GenericTypeArguments[2],
                        identityContext.GenericTypeArguments[3],
                        identityContext.GenericTypeArguments[4]);
                }
                services.TryAddTransient(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            }
        }
        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
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
