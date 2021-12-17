using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add
{
    public class AddClaimToUserCommand : IRequest<bool>
    {
        public string ClaimId { get; set; }
        public string UserId { get; set; }
    }
}