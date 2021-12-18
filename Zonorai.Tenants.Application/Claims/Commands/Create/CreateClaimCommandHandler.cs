using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.Domain.Claims;

namespace Zonorai.Tenants.Application.Claims.Commands.Create
{

    public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, Result>
    {
        private readonly ITenantDbContext _tenantDbContext;

        public CreateClaimCommandHandler(ITenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public async Task<Result> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var claim = new SecurityClaim(request.Type, request.Value);
                _tenantDbContext.Claims.Add(claim);
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