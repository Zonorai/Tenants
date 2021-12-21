using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;

public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string TenantId { get; set; }
}