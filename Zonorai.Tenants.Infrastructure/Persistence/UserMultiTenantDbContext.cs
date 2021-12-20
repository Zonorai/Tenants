using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Infrastructure.Persistence
{
    public abstract class UserMultiTenantDbContext : MultiTenantDbContext
    {
        protected UserMultiTenantDbContext(ITenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        protected UserMultiTenantDbContext(ITenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
        {
        }
        
        protected DbSet<User> Users { get; set; }
        protected DbSet<SecurityClaim> Claims { get; set; }
        protected DbSet<TenantInformation> TenantInfo { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
            base.OnModelCreating(builder);
        }
    }
}