using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserByEmail;

namespace Zonorai.Tenants.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    private readonly ITenantInfo _tenantInfo;
    private readonly ITenantDbContext _tenantDbContext;

    public GetUserByEmailQueryHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
    {
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x => x.Tenants)
            .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken: cancellationToken);
        
        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id))
        {
            return new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                EmailConfirmed = false
            };
        }

        throw new Exception("You do not have permissions to view this user");
    }
}