using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zonorai.Tenants.Infrastructure.Persistence;

public class TenantDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TenantDbContext>();
        builder.UseSqlServer(x => x.MigrationsAssembly(GetType().Assembly.FullName));
        var options = builder.Options;
        return TenantDbContext.Create(options);
    }
}