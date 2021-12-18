using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Zonorai.Tenants.ApplicationInterface;

public static class DependencyInjection
{
    public static IServiceCollection AddTenantApplicationInterface(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}