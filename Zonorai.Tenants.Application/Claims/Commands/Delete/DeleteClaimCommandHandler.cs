using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete;
using Zonorai.Tenants.Domain.Claims;

namespace Zonorai.Tenants.Application.Claims.Commands.Delete
{

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, Result>
    {
        private readonly ITenantDbContext _tenantDbContext;
        private readonly IEventStore _eventStore;
        private readonly ITenantInfo _tenantInfo;
        public DeleteClaimCommandHandler(ITenantDbContext tenantDbContext,IEventStore eventStore, ITenantInfo tenantInfo)
        {
            _tenantDbContext = tenantDbContext;
            _eventStore = eventStore;
            _tenantInfo = tenantInfo;
        }

        public async Task<Result> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_tenantInfo.Id))
                {
                    return Result.Fail("A session is required to modify this resource");
                }
                var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                if (claim.TenantId != _tenantInfo.Id)
                {
                    return Result.Fail("You do not have permissions to modify this resource");
                }
                _tenantDbContext.Claims.Remove(claim);
                await _eventStore.AddEvent(new ClaimDeletedEvent(claim.Id,DateTime.Now));
                await _tenantDbContext.SaveChangesAsync(cancellationToken);
                
                return Result.Ok();
            }
            catch(Exception e)
            {
                return Result.Fail(e.Message);
            }
        }
    }
}