using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create
{
    public class CreateClaimCommand : IRequest<bool>
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
    
}