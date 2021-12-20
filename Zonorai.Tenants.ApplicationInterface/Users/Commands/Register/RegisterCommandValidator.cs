using FluentValidation;
using Zonorai.Tenants.ApplicationInterface.Common;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Company).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Surname).NotEmpty().NotNull();
        RuleFor(x => x.Website).NotEmpty().NotNull();
        RuleFor(x => x.Password).Password();
    }
}