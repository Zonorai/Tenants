using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest<Result>
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
}