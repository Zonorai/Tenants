using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Users.Commands.Add;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand,Result>
{
    private readonly IUserService _userService;

    public AddUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        CreateUser createUser = new CreateUser(request.Email, request.Name, request.Surname, request.Password,
            request.PhoneNumber);
        return await _userService.AddUser(createUser);
    }
}