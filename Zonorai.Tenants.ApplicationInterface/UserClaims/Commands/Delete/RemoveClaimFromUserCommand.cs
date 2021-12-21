using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Delete;

public class RemoveClaimFromUserCommand : IRequest<Result>
{
    public string ClaimId { get; set; }
    public string UserId { get; set; }
}