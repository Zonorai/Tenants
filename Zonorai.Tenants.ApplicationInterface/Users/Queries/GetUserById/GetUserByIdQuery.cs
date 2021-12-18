using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public string Id { get; set; }
}