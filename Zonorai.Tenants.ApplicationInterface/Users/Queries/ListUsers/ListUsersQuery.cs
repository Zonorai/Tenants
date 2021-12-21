using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Users.Queries.ListUsers;

public class ListUsersQuery : IRequest<List<UserDto>>
{
}