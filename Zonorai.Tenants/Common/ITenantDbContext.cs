using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Claims;
using Zonorai.Tenants.Users;

namespace Zonorai.Tenants.Common
{
    public interface ITenantDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityClaim> Claims { get; set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}