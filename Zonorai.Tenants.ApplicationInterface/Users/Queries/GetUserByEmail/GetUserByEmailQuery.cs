using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserByEmail;

public class GetUserByEmailQuery : IRequest<UserDto>
{
    public string Email { get; set; }
}