using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zonorai.Tenants.Infrastructure.Persistence;

public class TenantDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        if (args.First() == "SQLSERVER")
        {
            var builder = new DbContextOptionsBuilder<TenantDbContext>();
            builder.UseSqlServer(x => x.MigrationsAssembly("Zonorai.Tenants.Migrations.SqlServer"));
            var options = builder.Options;
            return TenantDbContext.Create(options);
        }
        if (args.First() == "POSTGRESQL")
        {
            var builder = new DbContextOptionsBuilder<TenantDbContext>();
            builder.UseNpgsql(x => x.MigrationsAssembly("Zonorai.Tenants.Migrations.PostgreSQL"));
            var options = builder.Options;
            return TenantDbContext.Create(options);
        }
        if (args.First() == "SQLLITE")
        {
            var builder = new DbContextOptionsBuilder<TenantDbContext>();
            builder.UseSqlite(x => x.MigrationsAssembly("Zonorai.Tenants.Migrations.SqlLite"));
            var options = builder.Options;
            return TenantDbContext.Create(options);
        }

        throw new Exception("Provider not supported, Supported arguments are: SQLSERVER,SQLLITE,POSTGRESQL");
    }
}