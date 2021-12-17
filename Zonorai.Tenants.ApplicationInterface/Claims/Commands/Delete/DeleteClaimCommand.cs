using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete
{
    public class DeleteClaimCommand : IRequest<bool>
    {
        public string Id { get; set; }
    }
    
}