using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Claims;

namespace Zonorai.Tenants.Application.Claims.Commands.Delete
{
    public class DeleteClaimCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, bool>
    {
        private readonly ITenantDbContext _tenantDbContext;

        public DeleteClaimCommandHandler(ITenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public async Task<bool> Handle(DeleteClaimCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var claim = await _tenantDbContext.Claims.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                _tenantDbContext.Claims.Remove(claim);
                await _tenantDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}