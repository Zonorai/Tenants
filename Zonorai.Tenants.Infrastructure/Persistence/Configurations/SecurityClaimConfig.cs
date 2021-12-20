using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations
{
    public class SecurityClaimConfig : IEntityTypeConfiguration<SecurityClaim>
    {
        public void Configure(EntityTypeBuilder<SecurityClaim> builder)
        {
            builder.HasMany<UserClaim>().WithOne(x => x.Claim).HasForeignKey(x => x.ClaimId);
            builder.IsMultiTenant();
            builder.ToTable("Claims");
        }
    }
}