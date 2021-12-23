using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zonorai.Tenants.Application;
using Zonorai.Tenants.Application.Common.Configuration;
using Zonorai.Tenants.ApplicationInterface;
using Zonorai.Tenants.Infrastructure;
using Zonorai.Tenants.Infrastructure.Configuration;

namespace Zonorai.Tenants
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddZonoraiMultiTenancy(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTenantsApplication(configuration);
            services.AddTenantsInfrastructure(configuration);
            services.AddTenantsApplicationInterface();
            return services;
        }

        public static IServiceCollection AddZonoraiMultiTenancy(this IServiceCollection services,
            IConfiguration configuration, Action<TenantInfrastructureConfiguration> tenantInfrastructureConfiguration)
        {
            services.AddTenantsApplication(configuration);
            services.AddTenantsInfrastructure(tenantInfrastructureConfiguration);
            services.AddTenantsApplicationInterface();
            return services;
        }

        public static IServiceCollection AddZonoraiMultiTenancy(this IServiceCollection services,
            IConfiguration configuration, Action<TenantApplicationConfiguration> tenantApplicationConfiguration)
        {
            services.AddTenantsApplication(tenantApplicationConfiguration);
            services.AddTenantsInfrastructure(configuration);
            services.AddTenantsApplicationInterface();
            return services;
        }

        public static IServiceCollection AddZonoraiMultiTenancy(this IServiceCollection services,
            Action<TenantApplicationConfiguration> tenantApplicationConfiguration,
            Action<TenantInfrastructureConfiguration> tenantInfrastructureConfiguration)
        {
            services.AddTenantsApplication(tenantApplicationConfiguration);
            services.AddTenantsInfrastructure(tenantInfrastructureConfiguration);
            services.AddTenantsApplicationInterface();
            return services;
        }
    }
}