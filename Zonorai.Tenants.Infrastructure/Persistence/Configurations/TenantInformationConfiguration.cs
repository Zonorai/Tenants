using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Tenants;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations;

public class TenantInformationConfiguration : IEntityTypeConfiguration<TenantInformation>
{
    public void Configure(EntityTypeBuilder<TenantInformation> builder)
    {
        builder.HasKey(ti => ti.Id);
        builder.Property(ti => ti.Id).HasMaxLength(64);
        builder.HasIndex(ti => ti.Identifier).IsUnique();
        builder.HasMany(x => x.Users).WithMany(x => x.Tenants);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.ToTable("TenantInfo");
    }
}