using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Add
{
    public class AddClaimToUserCommand : IRequest<bool>
    {
        public string ClaimId { get; set; }
        public string UserId { get; set; }
    }

    public class AddClaimToUserCommandHandler : IRequestHandler<AddClaimToUserCommand, bool>
    {
        private readonly ITenantDbContext _tenantDbContext;
        private readonly ITenantInfo _tenantInfo;

        public AddClaimToUserCommandHandler(ITenantDbContext tenantDbContext, ITenantInfo tenantInfo)
        {
            _tenantDbContext = tenantDbContext;
            _tenantInfo = tenantInfo;
        }

        public async Task<bool> Handle(AddClaimToUserCommand request, CancellationToken cancellationToken)
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

            user.Claims.Add(new UserClaim(claim.Id, user.Id, _tenantInfo.Id));
            _tenantDbContext.Users.Update(user);
            await _tenantDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}