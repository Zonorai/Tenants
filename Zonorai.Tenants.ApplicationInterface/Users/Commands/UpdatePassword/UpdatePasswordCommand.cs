using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdatePassword;

public class UpdatePasswordCommand : IRequest<Result>
{
    public string UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}