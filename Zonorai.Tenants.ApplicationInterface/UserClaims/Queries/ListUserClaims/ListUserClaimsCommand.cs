using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Queries.ListUserClaims
{
    public class ListUserClaimsCommand : IRequest<List<UserClaimDto>>
    {
        public string UserId { get; set; }
    }
}