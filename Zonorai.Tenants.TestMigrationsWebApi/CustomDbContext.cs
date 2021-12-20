using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Domain.Users;
using Zonorai.Tenants.Infrastructure.Persistence;

namespace Zonorai.Tenants.WebApi;
/// <summary>
/// Testing integrating context into own context
/// </summary>
public class CustomDbContext : UserMultiTenantDbContext
{
    public CustomDbContext(ITenantInfo tenantInfo) : base(tenantInfo)
    {
    }

    public CustomDbContext(ITenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Something>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
        builder.Entity<Something>().HasKey(x => x.Id);
        base.OnModelCreating(builder);
    }
}

public class Something
{
    public string Id { get; set; }
    public string UserId { get; set; }
    
}