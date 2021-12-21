using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add;
using Zonorai.Tenants.Domain.UserClaims;

namespace Zonorai.Tenants.Application.UserClaims.Commands.Add;

public class AddClaimToUserCommandHandler : IRequestHandler<AddClaimToUserCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public AddClaimToUserCommandHandler(ITenantDbContext tenantDbContext, ITenantInfo tenantInfo,
        IEventStore eventStore)
    {
        _tenantDbContext = tenantDbContext;
        _tenantInfo = tenantInfo;
        _eventStore = eventStore;
    }

    public async Task<Result> Handle(AddClaimToUserCommand request, CancellationToken cancellationToken)
    {
        var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.ClaimId,
            cancellationToken);
        if (claim == null) return Result.Fail("Claim not found");

        var user = await _tenantDbContext.Users.Include(x => x.Tenants)
            .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            return Result.Fail("You do not have permissions to invoke any methods on this user");

        user.Claims.Add(new UserClaim(claim.Id, user.Id, _tenantInfo.Id));
        _tenantDbContext.Users.Update(user);
        await _eventStore.AddEvent(new ClaimAddedToUserEvent(claim.Id, user.Id, DateTime.Now));
        await _tenantDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}