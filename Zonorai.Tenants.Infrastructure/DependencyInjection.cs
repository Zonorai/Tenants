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
            var section = configuration.GetSection(nameof(TenantsConfiguration));
            if (section == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantsConfiguration)}");
            }

            var config = new TenantsConfiguration();
            section.Bind(config);
            services.Configure<TenantsConfiguration>(section);

            services.AddDbContext<TenantDbContext>(x =>
                x.UseSqlServer(section.GetValue<string>(nameof(TenantsConfiguration.DbConnection)), y =>
                {
                    if (migrationsAssembly != null)
                    {
                        y.MigrationsAssembly(migrationsAssembly.FullName);
                    }
                }));

            services.AddScoped<ITenantDbContext>(x => x.GetService<TenantDbContext>());

            services.AddMultiTenant<TenantInformation>().WithClaimStrategy(TenantConstants.TenantIdentifier)
                .WithEFCoreStore<TenantDbContext, TenantInformation>();

            services.AddTransient<ITokenService, TokenService>();

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireRole("Owner"));
            });
            
            return services;
        }
    }
}