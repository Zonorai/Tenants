using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.Claims.Queries;
using Zonorai.Tenants.ApplicationInterface.Claims.Queries.ListClaims;

namespace Zonorai.Tenants.Net6WebApi.Controllers;
[Authorize]
public class ClaimsController : ApiControllerBase
{
    [HttpPost]
    [Route("[action]")]
    public async Task<Result> Create([FromBody]CreateClaimCommand command)
    {
        var result = await Mediator.Send(command);
        return result;
    }
    [HttpDelete]
    [Route("[action]")]
    public async Task<Result> Delete([FromBody]DeleteClaimCommand command)
    {
        var result = await Mediator.Send(command);
        return result;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<List<ClaimDto>> List([FromQuery] ListClaimsQuery query)
    {
        var result = await Mediator.Send(query);
        return result;
    }
}