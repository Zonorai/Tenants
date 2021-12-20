using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string UserId { get; set; }
    public string Password { get; set; }
}