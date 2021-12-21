using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.ResetPassword;

namespace Zonorai.Tenants.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public ResetPasswordCommandHandler(ITenantInfo tenantInfo, ITenantDbContext tenantDbContext, IEventStore eventStore)
    {
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
        _eventStore = eventStore;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x => x.Tenants)
            .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            return Result.Fail("You don't have permissions to edit this user");

        user.SetPassword(request.Password);
        _tenantDbContext.Users.Update(user);
        await _eventStore.AddEvent(new PasswordResetEvent(user.Id, DateTime.Now));
        await _tenantDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}