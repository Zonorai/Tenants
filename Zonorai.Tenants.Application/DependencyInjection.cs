using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Zonorai.Tenants.Application.Claims.Commands.Create;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;

namespace Zonorai.Tenants.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddZonoraiTenantApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateClaimCommand).Assembly);
            
            return services;
        }
    }
}