using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;

namespace Zonorai.Tenants.Application.Claims.Queries.ListClaims
{
    public class ListClaimsQuery : IRequest<List<Claim>>
    {
        
    }

    public class ListClaimsQueryHandler : IRequestHandler<ListClaimsQuery, List<Claim>>
    {
        private readonly ITenantDbContext _tenantDbContext;

        public ListClaimsQueryHandler(ITenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public async Task<List<Claim>> Handle(ListClaimsQuery request, CancellationToken cancellationToken)
        {
            var claims = await _tenantDbContext.Claims.ToListAsync(cancellationToken: cancellationToken);
            List<Claim> claimsToReturn = new List<Claim>();
            claims.ForEach(x=> claimsToReturn.Add(new Claim(x.Type,x.Value)));
            return claimsToReturn;
        }
    }
}