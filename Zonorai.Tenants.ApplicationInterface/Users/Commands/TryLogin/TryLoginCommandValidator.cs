using FluentValidation;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;

public class TryLoginCommandValidator : AbstractValidator<TryLoginCommand>
{
    public TryLoginCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x=> x.Password).Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$").WithMessage(
            "Password must be minimum eight characters, at least one letter, one number and one special character");
    }
}