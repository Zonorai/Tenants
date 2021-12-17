using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Infrastructure.Persistence
{
    public class TenantDbContext : EFCoreStoreDbContext<TenantInformation>,ITenantDbContext
    {
        private IMediator _mediator;
        
        public TenantDbContext(DbContextOptions<TenantDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityClaim> Claims { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}