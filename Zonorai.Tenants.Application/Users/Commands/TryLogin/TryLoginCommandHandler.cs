using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;

namespace Zonorai.Tenants.Application.Users.Commands.TryLogin;

public class TryLoginCommandHandler : IRequestHandler<TryLoginCommand, LoginResult>
{
    private readonly IUserService _userService;

    public TryLoginCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<LoginResult> Handle(TryLoginCommand request, CancellationToken cancellationToken)
    {
        return await _userService.Login(request.Email, request.Password);
    }
}