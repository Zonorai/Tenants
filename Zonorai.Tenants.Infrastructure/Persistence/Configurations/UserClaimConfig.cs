using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations
{
    public class UserClaimConfig : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            
        }
    }
}