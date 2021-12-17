using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Claims;
using Zonorai.Tenants.Common;
using Zonorai.Tenants.Entities;
using Zonorai.Tenants.Users;

namespace Zonorai.Tenants
{
    public class TenantDbContext : EFCoreStoreDbContext<TenantInformation>,ITenantDbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityClaim> Claims { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}