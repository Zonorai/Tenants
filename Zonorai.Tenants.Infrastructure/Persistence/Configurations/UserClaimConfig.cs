using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations;

public class UserClaimConfig : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.HasKey(x => new {x.UserId, x.ClaimId, x.TenantId});
        builder.HasOne<SecurityClaim>(x=> x.Claim).WithMany().HasForeignKey(x => x.ClaimId);
        builder.IsMultiTenant();
        builder.ToTable("UserClaims");
    }
}