using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Claims;

namespace Zonorai.Tenants.Application.Claims.Commands.Create
{
    public class CreateClaimCommand : IRequest<bool>
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class CreateClaimCommandHandler : IRequestHandler<CreateClaimCommand, bool>
    {
        private readonly ITenantDbContext _tenantDbContext;

        public CreateClaimCommandHandler(ITenantDbContext tenantDbContext)
        {
            _tenantDbContext = tenantDbContext;
        }

        public async Task<bool> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var claim = new SecurityClaim(request.Type, request.Value);
                _tenantDbContext.Claims.Add(claim);
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