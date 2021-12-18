using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Delete;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Delete
{

    public class RemoveClaimFromUserCommandHandler : IRequestHandler<RemoveClaimFromUserCommand, Result>
    {
        private readonly ITenantDbContext _tenantDbContext;
        private readonly ITenantInfo _tenantInfo;

        public RemoveClaimFromUserCommandHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
        {
            _tenantInfo = tenantInfo;
            _tenantDbContext = tenantDbContext;
        }

        public async Task<Result> Handle(RemoveClaimFromUserCommand request, CancellationToken cancellationToken)
        {
            var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.ClaimId,
                cancellationToken);
            if (claim == null)
            {
                return Result.Fail("Claim not found");
            }

            var user = await _tenantDbContext.Users.Include(x => x.Tenants)
                .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            {
                return Result.Fail("You do not have permissions to invoke any methods on this user");
            }

            var userClaim = user.Claims.SingleOrDefault(x => x.ClaimId == request.ClaimId);
            user.Claims.Remove(userClaim);
            _tenantDbContext.Users.Update(user);
            await _tenantDbContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}