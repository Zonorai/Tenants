using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Queries;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Queries.ListUserClaims;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdatePassword;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdateUserDetails;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserByEmail;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.GetUserById;
using Zonorai.Tenants.ApplicationInterface.Users.Queries.ListUsers;

namespace Zonorai.Tenants.Net6WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
}

public class UserController : ApiControllerBase
{
    [HttpPost]
    [Route("[action]")]
    [Authorize]
    public async Task<ActionResult<Result>> ConfirmEmail([FromBody] ConfirmUserEmailCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }
    [HttpPost]
    [Route("[action]")]
    [Authorize]
    public async Task<ActionResult<Result>> AddClaimToUser([FromBody] AddClaimToUserCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }
    [HttpPost]
    [Route("[action]")]
    [Authorize]
    public async Task<ActionResult<Result>> RemoveClaimFromUser([FromBody] RemoveClaimFromUserCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }
    [HttpPost]
    [Route("[action]")]
    [Authorize]
    public async Task<ActionResult<Result>> AddUser([FromBody] AddUserCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }
    
    [HttpPost]
    [Route("[action]")]
    [Authorize]
    public async Task<ActionResult<Result>> DeleteUser([FromBody] DeleteUserCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<LoginResult>> LoginToTenantDirectory([FromBody] LoginCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<LoginResult>> Login([FromBody] TryLoginCommand command)
    {
        var res = await Mediator.Send(command);

        return Ok(res);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }

    [Authorize]
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<Result>> UpdatePassword([FromBody] UpdatePasswordCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }

    [Authorize]
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<Result>> UpdateDetails([FromBody] UpdateUserDetailsCommand command)
    {
        var res = await Mediator.Send(command);
        return Ok(res);
    }

    [Authorize]
    [HttpGet]
    [Route("[action]")]
    public async Task<UserDto> GetUserByEmail([FromQuery] GetUserByEmailQuery query)
    {
        var res = await Mediator.Send(query);
        return res;
    }

    [Authorize]
    [HttpGet]
    [Route("[action]")]
    public async Task<UserDto> GetUserById([FromQuery] GetUserByIdQuery query)
    {
        var res = await Mediator.Send(query);
        return res;
    }

    [Authorize]
    [HttpGet]
    [Route("[action]")]
    public async Task<List<UserDto>> ListUsers([FromQuery] ListUsersQuery query)
    {
        var res = await Mediator.Send(query);
        return res;
    }
    [Authorize]
    [HttpGet]
    [Route("[action]")]
    public async Task<List<UserClaimDto>> ListUserClaims([FromQuery] ListUserClaimsQuery query)
    {
        var res = await Mediator.Send(query);
        return res;
    }
}