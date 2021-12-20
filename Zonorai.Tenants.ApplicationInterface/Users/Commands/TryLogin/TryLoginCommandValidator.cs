using FluentValidation;
using Zonorai.Tenants.ApplicationInterface.Common;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;

public class TryLoginCommandValidator : AbstractValidator<TryLoginCommand>
{
    public TryLoginCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x=> x.Password).Password();
    }
}