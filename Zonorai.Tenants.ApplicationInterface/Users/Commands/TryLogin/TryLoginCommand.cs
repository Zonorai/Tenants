using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;

public class TryLoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
}