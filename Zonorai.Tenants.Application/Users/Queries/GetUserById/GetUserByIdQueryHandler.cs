using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserById;

namespace Zonorai.Tenants.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public GetUserByIdQueryHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext)
    {
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x => x.Tenants)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id))
            return new UserDto
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber
            };

        throw new Exception("You do not have permissions to view this user");
    }
}