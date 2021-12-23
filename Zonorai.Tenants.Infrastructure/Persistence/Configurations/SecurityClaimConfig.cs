using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations;

public class SecurityClaimConfig : IEntityTypeConfiguration<SecurityClaim>
{
    public void Configure(EntityTypeBuilder<SecurityClaim> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x=> x.Value).IsRequired();
        builder.HasIndex(x => new {x.Value, x.Type,x.TenantId}).IsUnique();
        builder.IsMultiTenant();
        builder.ToTable("Claims");
    }
}