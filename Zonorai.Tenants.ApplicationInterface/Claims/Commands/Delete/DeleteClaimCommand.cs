using FluentResults;
using MediatR;

namespace Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete
{
    public class DeleteClaimCommand : IRequest<Result>
    {
        public string Id { get; set; }
    }
    
}