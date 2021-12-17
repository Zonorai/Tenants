using FluentValidation;
using Zonorai.Tenants.Claims;
using Zonorai.Tenants.Common;
using Zonorai.Tenants.Users;

namespace Zonorai.Tenants.UserClaims
{
    public class UserClaim
    {
        public UserClaim(string claimId, string userId, string tenantId)
        {
            FluentValueValidator<string>.Validate(claimId, x => x.NotEmpty().NotNull());
            FluentValueValidator<string>.Validate(userId, x => x.NotEmpty().NotNull());
            FluentValueValidator<string>.Validate(tenantId, x => x.NotEmpty().NotNull());
            ClaimId = claimId;
            UserId = userId;
            TenantId = tenantId;
        }

        public string ClaimId { get; private set; }

        public SecurityClaim Claim { get; private set; }
        public string UserId { get; private set; }

        public User User { get; private set; }
        public string TenantId { get; private set; }
    }
}