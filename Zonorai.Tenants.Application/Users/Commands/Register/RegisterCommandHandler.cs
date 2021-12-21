using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Application.Common.Configuration;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserService _userService;
    private readonly IOptions<TenantApplicationConfiguration> _options;

    public RegisterCommandHandler(IUserService userService, IOptions<TenantApplicationConfiguration> options)
    {
        _userService = userService;
        _options = options;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerTenant = new RegisterTenant(request.Company, request.Website, request.Email,
            request.Name, request.Surname, request.Password, request.PhoneNumber);
        var user = await _userService.RegisterTenant(registerTenant);

        if (user == null) return new RegisterResult(null, null, "Failed To Create User",false);

        if (_options.Value.RequireConfirmedEmailForLogin == false)
        {
            var result = await _userService.Login(user.Email, request.Password, user.Tenants.First().Id);

            if (string.IsNullOrWhiteSpace(result.ErrorMessage) == false)
            {
                return new RegisterResult(user.Id, null, result.ErrorMessage,false);
            }
            
            return new RegisterResult(user.Id, result.Token, null,false);
        }

        return new RegisterResult(user.Id, null, null,true);
    }
}