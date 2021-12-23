using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Infrastructure.Persistence;

public class TenantStoreDbContext : EFCoreStoreDbContext<TenantInformation>
{
    public TenantStoreDbContext(DbContextOptions options) : base(options)
    {
    }
    
    internal DbSet<SecurityClaim> Claims { get; set; }
    internal DbSet<UserClaim> UserClaims { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}