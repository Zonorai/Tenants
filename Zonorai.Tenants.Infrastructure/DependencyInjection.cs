using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Infrastructure.Configuration;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.Infrastructure.Services;

namespace Zonorai.Tenants.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTenantsInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(TenantInfrastructureConfiguration));
        if (section == null)
            throw new InvalidOperationException(
                $"Cannot add Multi Tenancy without the configuration for type {nameof(TenantInfrastructureConfiguration)}");

        var config = new TenantInfrastructureConfiguration();
        section.Bind(config);
        services.Configure<TenantInfrastructureConfiguration>(section);
        services.AddInfrastructure(config);
        return services;
    }

    public static IServiceCollection AddTenantsInfrastructure(this IServiceCollection services,
        Action<TenantInfrastructureConfiguration> configurationAction)
    {
        var config = new TenantInfrastructureConfiguration();
        configurationAction.Invoke(config);
        services.Configure(configurationAction);
        services.AddInfrastructure(config);
        return services;
    }

    private static IServiceCollection AddInfrastructure(this IServiceCollection services,
        TenantInfrastructureConfiguration config)
    {
        switch (config.Provider)
        {
            case TenantsProvider.SqlServer:
                services.AddDbContext<TenantDbContext>(x =>
                    x.UseSqlServer(config.DbConnection,
                        y => { y.MigrationsAssembly("Zonorai.Tenants.Migrations.SqlServer"); }));
                services.AddDbContext<TenantStoreDbContext>(x =>
                    x.UseSqlServer(config.DbConnection));
                break;

            case TenantsProvider.PostgreSql:
                services.AddDbContext<TenantDbContext>(x =>
                    x.UseNpgsql(config.DbConnection,
                        y => { y.MigrationsAssembly("Zonorai.Tenants.Migrations.PostgreSQL"); }));
                services.AddDbContext<TenantStoreDbContext>(x =>
                    x.UseNpgsql(config.DbConnection));
                break;

            case TenantsProvider.Sqlite:
                if (config.DbConnection.Contains("Filename=:memory:") ||
                    config.DbConnection.Contains("Data Source=:memory:"))
                {
                    var inMemorySqlite = new SqliteConnection("Filename=:memory:");
                    inMemorySqlite.Open();
                    services.AddDbContext<TenantDbContext>(x =>
                        x.UseSqlite(inMemorySqlite,
                            y => { y.MigrationsAssembly("Zonorai.Tenants.Migrations.SqlLite"); }));
                    services.AddDbContext<TenantStoreDbContext>(x =>
                        x.UseSqlite(inMemorySqlite));
                }
                else
                {
                    services.AddDbContext<TenantDbContext>(x =>
                        x.UseSqlite(config.DbConnection,
                            y => { y.MigrationsAssembly("Zonorai.Tenants.Migrations.SqlLite"); }));
                    services.AddDbContext<TenantStoreDbContext>(x =>
                        x.UseSqlite(config.DbConnection));
                }

                break;
        }


        services.AddScoped<ITenantDbContext>(x => x.GetService<TenantDbContext>());

        services.AddMultiTenant<TenantInformation>().WithClaimStrategy(TenantConstants.TenantIdentifier)
            .WithEFCoreStore<TenantStoreDbContext, TenantInformation>();

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
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = config.SymmetricSecurityKey,
                        ValidateIssuer = !string.IsNullOrWhiteSpace(config.ValidIssuer),
                        ValidateAudience = !string.IsNullOrWhiteSpace(config.ValidAudience),
                        ValidAudience = config.ValidAudience,
                        ValidIssuer = config.ValidIssuer,
                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        return services;
    }
}