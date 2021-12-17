using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;

namespace Zonorai.Tenants.Application.Users.Commands.Login
{
    public class LoginCommand : IRequest<LoginResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TenantId { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IUserService _userService;

        public LoginCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _userService.Login(request.Email, request.Password,request.TenantId);
        }
    }
}