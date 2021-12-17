using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Users.Commands.Register
{
    public class RegisterCommand : IRequest<LoginResult>
    {
        public string Company { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand,LoginResult>
    {
        private readonly IUserService _userService;

        public RegisterCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<LoginResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var registerTenant = new RegisterTenant(request.Company, request.Website, request.Email, request.Username,
                request.Name, request.Surname, request.Password);
            var user = await _userService.RegisterTenant(registerTenant);
            return await _userService.Login(user.Email, user.Password);
        }
    }
}