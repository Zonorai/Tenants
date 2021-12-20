using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Delete;

public class DeleteUserCommand : IRequest<Result>
{
    public string UserId { get; set; }
}