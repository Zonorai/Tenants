using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;

namespace Zonorai.Tenants.Application.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public LoginCommandHandler(IUserService userService, IMediator mediator)
    {
        _userService = userService;
        _mediator = mediator;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.Login(request.Email, request.Password, request.TenantId);

        return result;
    }
}