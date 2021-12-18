using FluentValidation;

namespace Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add;

public class AddClaimToUserCommandValidator : AbstractValidator<AddClaimToUserCommand>
{
    public AddClaimToUserCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotNull().NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
    }
}