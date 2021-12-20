using FluentValidation;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Delete;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty();
    }
}