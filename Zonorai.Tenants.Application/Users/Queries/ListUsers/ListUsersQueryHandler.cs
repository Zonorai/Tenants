using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.ListUsers;

namespace Zonorai.Tenants.Application.Users.Queries.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, List<UserDto>>
{
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public ListUsersQueryHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
    {
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<List<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users1 = await _tenantDbContext.Users.Include(x => x.Tenants).ToListAsync();
        var users = await _tenantDbContext.Users.Where(x => x.Tenants.Any(x => x.Id == _tenantInfo.Id))
            .ToListAsync(cancellationToken);
        
        var userDtos = new List<UserDto>();
        
        foreach (var user in users)
            userDtos.Add(new UserDto
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber
            });

        return userDtos;
    }
}