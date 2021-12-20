using FluentValidation;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;

public class ConfirmUserEmailValidator : AbstractValidator<ConfirmUserEmailCommand>
{
    public ConfirmUserEmailValidator()
    {
        RuleFor(x=> x.UserId).NotNull().NotEmpty();
    }
}