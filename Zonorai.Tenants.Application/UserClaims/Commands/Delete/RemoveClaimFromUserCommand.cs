using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Delete
{
    public class RemoveClaimFromUserCommand : IRequest<bool>
    {
        public string ClaimId { get; set; }
        public string UserId { get; set; }
    }

    public class RemoveClaimFromUserCommandHandler : IRequestHandler<RemoveClaimFromUserCommand, bool>
    {
        private readonly ITenantDbContext _tenantDbContext;
        private readonly ITenantInfo _tenantInfo;

        public RemoveClaimFromUserCommandHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
        {
            _tenantInfo = tenantInfo;
            _tenantDbContext = tenantDbContext;
        }

        public async Task<bool> Handle(RemoveClaimFromUserCommand request, CancellationToken cancellationToken)
        {
            var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.ClaimId,
                cancellationToken);
            if (claim == null)
            {
                throw new Exception("Claim not found");
            }

            var user = await _tenantDbContext.Users.Include(x => x.Tenants)
                .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            {
                throw new Exception("You do not have permissions to invoke any methods on this user");
            }

            var userClaim = user.Claims.SingleOrDefault(x => x.ClaimId == request.ClaimId);
            user.Claims.Remove(userClaim);
            _tenantDbContext.Users.Update(user);
            await _tenantDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}