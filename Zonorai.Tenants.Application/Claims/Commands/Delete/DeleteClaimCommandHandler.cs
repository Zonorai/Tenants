using System;
using System.Threading;
using System.Threading.Tasks;
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

        public DeleteClaimCommandHandler(ITenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public async Task<Result> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                _tenantDbContext.Claims.Remove(claim);
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