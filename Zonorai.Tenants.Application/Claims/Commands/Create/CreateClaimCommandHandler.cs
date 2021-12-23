using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.Domain.Claims;

namespace Zonorai.Tenants.Application.Claims.Commands.Create;

public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;
    public CreateClaimCommandHandler(ITenantDbContext tenantDbContext, IEventStore eventStore, ITenantInfo tenantInfo)
    {
        _tenantDbContext = tenantDbContext;
        _eventStore = eventStore;
        _tenantInfo = tenantInfo;
    }

    public async Task<Result> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (_tenantInfo == null)
            {
                return Result.Fail("A session is required to modify a claim");
            }
            var claim = new SecurityClaim(request.Type, request.Value);
            _tenantDbContext.Claims.Add(claim);
            await _eventStore.AddEvent(new ClaimCreatedEvent(claim.Id, claim.Value, claim.Type, DateTime.Now));
            await _tenantDbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}