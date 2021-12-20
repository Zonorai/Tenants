using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;

public class AddUserCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}