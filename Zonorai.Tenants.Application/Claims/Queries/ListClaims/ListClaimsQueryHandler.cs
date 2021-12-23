using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Claims.Queries;
using Zonorai.Tenants.ApplicationInterface.Claims.Queries.ListClaims;

namespace Zonorai.Tenants.Application.Claims.Queries.ListClaims;

public class ListClaimsQueryHandler : IRequestHandler<ListClaimsQuery, List<ClaimDto>>
{
    private readonly ITenantDbContext _tenantDbContext;

    public ListClaimsQueryHandler(ITenantDbContext tenantDbContext)
    {
        _tenantDbContext = tenantDbContext;
    }

    public async Task<List<ClaimDto>> Handle(ListClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await _tenantDbContext.Claims.ToListAsync(cancellationToken);
        var claimsToReturn = new List<ClaimDto>();
        claims.ForEach(x => claimsToReturn.Add(new ClaimDto(){Type = x.Type, Value = x.Value,Id = x.Id}));
        return claimsToReturn;
    }
}