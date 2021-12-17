using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Common
{
    public interface ITenantDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityClaim> Claims { get; set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}