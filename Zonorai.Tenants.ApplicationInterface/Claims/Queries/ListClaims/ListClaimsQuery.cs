using System.Security.Claims;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Claims.Queries.ListClaims;

public class ListClaimsQuery : IRequest<List<ClaimDto>>
{
}