using FluentValidation;

namespace Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;

public class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.Type).NotEmpty().NotNull();
        RuleFor(x => x.Value).NotEmpty().Null();
    }
}