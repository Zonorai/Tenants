using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete;

namespace Zonorai.Tenants.Application.Claims.Commands.Delete;

public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;
    public DeleteClaimCommandHandler(ITenantDbContext tenantDbContext, IEventStore eventStore, ITenantInfo tenantInfo)
    {
        _tenantDbContext = tenantDbContext;
        _eventStore = eventStore;
        _tenantInfo = tenantInfo;
    }

    public async Task<Result> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (_tenantInfo == null)
            {
                return Result.Fail("A session is required to modify a claim");
            }
            var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.Id,
                cancellationToken);
            _tenantDbContext.Claims.Remove(claim);
            await _eventStore.AddEvent(new ClaimDeletedEvent(claim.Id, DateTime.Now));
            await _tenantDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}