using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add;

public class AddClaimToUserCommand : IRequest<Result>
{
    public string ClaimId { get; set; }
    public string UserId { get; set; }
}