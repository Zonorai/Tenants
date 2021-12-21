using FluentValidation;
using Zonorai.Tenants.ApplicationInterface.Common;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).Password();
        RuleFor(x => x.TenantId).NotEmpty().NotNull();
    }
}