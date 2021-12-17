using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zonorai.Tenants.Common;
using Zonorai.Tenants.Entities;

namespace Zonorai.Tenants
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMultiTenancy(this IServiceCollection services,
            IConfiguration configuration, Assembly migrationsAssembly)
        {
            var section = configuration.GetSection(nameof(TenantsConfiguration));
            if (section == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantsConfiguration)}");
            }
            services.Configure<TenantsConfiguration>(section);
            
            services.AddDbContext<TenantDbContext>(x =>
                x.UseSqlServer(section.GetValue<string>(nameof(TenantsConfiguration.DbConnection)), 
                    y => y.MigrationsAssembly(migrationsAssembly.FullName)));
            
            services.AddScoped<ITenantDbContext>(x => x.GetService<TenantDbContext>());
            
            services.AddMultiTenant<TenantInformation>().WithClaimStrategy(TenantConstants.TenantIdentifier)
                .WithEFCoreStore<TenantDbContext, TenantInformation>();
            
            services.AddTransient<ITokenService, TokenService>();
            
            return services;
        }
    }
}