using System;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Application.Common.Behaviours;
using Zonorai.Tenants.ApplicationInterface;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;

namespace Zonorai.Tenants.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTenantsApplication(this IServiceCollection services,IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(TenantApplicationConfiguration));
            
            if (section == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantApplicationConfiguration)}");
            }
            
            services.Configure<TenantApplicationConfiguration>(section);
            services.AddApplication();
            return services;
        }
        public static IServiceCollection AddTenantsApplication(this IServiceCollection services,Action<TenantApplicationConfiguration> configuration)
        {
            services.Configure<TenantApplicationConfiguration>(configuration);
            services.AddApplication();
            return services;
        }
        private static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddTenantsApplicationInterface();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledResultExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            return services;
        }
    }
}