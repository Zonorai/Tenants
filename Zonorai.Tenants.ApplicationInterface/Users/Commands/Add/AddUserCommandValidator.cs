using System.Data;
using FluentValidation;
using Zonorai.Tenants.ApplicationInterface.Common;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Surname).NotEmpty().NotNull();
        RuleFor(x => x.Password).Password();
    }
}