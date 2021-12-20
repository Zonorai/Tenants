using System;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Infrastructure.Configuration;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.Infrastructure.Services;

namespace Zonorai.Tenants.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTenantsInfrastructure(this IServiceCollection services,
            IConfiguration configuration, Assembly migrationsAssembly = null)
        {
            var section = configuration.GetSection(nameof(TenantInfrastructureConfiguration));
            if (section == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantInfrastructureConfiguration)}");
            }

            var config = new TenantInfrastructureConfiguration();
            section.Bind(config);
            services.Configure<TenantInfrastructureConfiguration>(section);

            return services;
        }

        public static IServiceCollection AddTenantsInfrastructure(this IServiceCollection services,
            Action<TenantInfrastructureConfiguration> configurationAction, Assembly migrationsAssembly = null)
        {
            var config = new TenantInfrastructureConfiguration();
            configurationAction.Invoke(config);
            services.Configure<TenantInfrastructureConfiguration>(configurationAction);
            services.AddInfrastructure(config);
            return services;
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services,
            TenantInfrastructureConfiguration config, Assembly migrationsAssembly = null)
        {
            services.AddDbContext<TenantDbContext>(x =>
                x.UseSqlServer(config.DbConnection, y =>
                {
                    if (migrationsAssembly != null)
                    {
                        y.MigrationsAssembly(migrationsAssembly.FullName);
                    }

                    if (migrationsAssembly == null)
                    {
                        y.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                    }
                }));

            services.AddScoped<ITenantDbContext>(x => x.GetService<TenantDbContext>());

            services.AddMultiTenant<TenantInformation>().WithClaimStrategy(TenantConstants.TenantIdentifier)
                .WithEFCoreStore<TenantDbContext, TenantInformation>();

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IEventStore, EventStore>();
            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(auth =>
                {
                    auth.RequireHttpsMetadata = false;
                    auth.SaveToken = false;
                    auth.TokenValidationParameters =
                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = config.SymmetricSecurityKey,
                            ValidateIssuer = !string.IsNullOrWhiteSpace(config.ValidIssuer),
                            ValidateAudience = !string.IsNullOrWhiteSpace(config.ValidAudience),
                            ValidAudience = config.ValidAudience,
                            ValidIssuer = config.ValidIssuer,
                            ClockSkew = TimeSpan.Zero,
                        };
                });

            services.AddAuthorization();

            return services;
        }
    }
}