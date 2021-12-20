
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.UserClaims;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations
{
    public class UserClaimConfig : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(x => new {x.UserId, x.ClaimId, x.TenantId});
            builder.ToTable("UserClaims");
        }
    }
}