using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Users.Commands.Register
{

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand,RegisterResult>
    {
        private readonly IUserService _userService;

        public RegisterCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var registerTenant = new RegisterTenant(request.Company, request.Website, request.Email, request.Username,
                request.Name, request.Surname, request.Password);
            var user = await _userService.RegisterTenant(registerTenant);
            
            if (user == null)
            {
                return new RegisterResult(null, null, "Failed To Create User");
            }
            
            var result = await _userService.Login(user.Email, user.Password);
            
            if (string.IsNullOrWhiteSpace(result.ErrorMessage) == false)
            {
                return new RegisterResult(user.Id, result.Token, null);
            }
            return new RegisterResult(user.Id, result.Token, null);
        }
    }
}