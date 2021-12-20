using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Infrastructure.Persistence.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Ignore(x => x.FullName);
            builder.HasMany<UserClaim>(x => x.Claims).WithOne(x => x.User).HasForeignKey(x=> x.UserId);
            
            builder.HasIndex(x => x.Email).IsUnique();
            builder.ToTable("Users");
        }
    }
}