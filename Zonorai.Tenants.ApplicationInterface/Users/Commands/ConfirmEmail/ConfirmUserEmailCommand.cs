using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;

public class ConfirmUserEmailCommand : IRequest<Result>
{
    public string UserId { get; set; }
}