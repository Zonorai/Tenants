using FluentValidation;
using Zonorai.Tenants.ApplicationInterface.Common;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x=> x.CurrentPassword).Password();
        RuleFor(x=> x.NewPassword).Password();
        RuleFor(x => x.UserId).NotEmpty().NotNull();
    }
}