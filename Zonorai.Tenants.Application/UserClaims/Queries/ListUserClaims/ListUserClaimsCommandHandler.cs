using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Queries;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Queries.ListUserClaims;

namespace Zonorai.Tenants.Application.UserClaims.Queries.ListUserClaims;

public class ListUserClaimsCommandHandler : IRequestHandler<ListUserClaimsCommand, List<UserClaimDto>>
{
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public ListUserClaimsCommandHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
    {
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<List<UserClaimDto>> Handle(ListUserClaimsCommand request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x => x.Tenants).Include(x => x.Claims)
            .ThenInclude(x => x.Claim)
            .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            throw new Exception("You do not have permissions to invoke any methods on this user");

        var userClaims = new List<UserClaimDto>();
        foreach (var claim in user.Claims)
            userClaims.Add(new UserClaimDto
            {
                UserId = claim.UserId,
                ClaimId = claim.ClaimId,
                Type = claim.Claim.Type,
                Value = claim.Claim.Value
            });

        return userClaims;
    }
}