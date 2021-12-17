using System;
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
        public static IServiceCollection AddZonoraiTenantsInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(TenantsConfiguration));
            if (section == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantsConfiguration)}");
            }
            services.Configure<TenantsConfiguration>(section);
            
            services.AddDbContext<TenantDbContext>(x =>
                x.UseSqlServer(section.GetValue<string>(nameof(TenantsConfiguration.DbConnection))));
            
            services.AddScoped<ITenantDbContext>(x => x.GetService<TenantDbContext>());
            
            services.AddMultiTenant<TenantInformation>().WithClaimStrategy(TenantConstants.TenantIdentifier)
                .WithEFCoreStore<TenantDbContext, TenantInformation>();
            
            services.AddTransient<ITokenService, TokenService>();
            
            return services;
        }
    }
}