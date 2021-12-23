using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;

namespace Zonorai.Tenants.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmUserEmailCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;

    public ConfirmEmailCommandHandler(ITenantDbContext tenantDbContext, IEventStore eventStore)
    {
        _tenantDbContext = tenantDbContext;
        _eventStore = eventStore;
    }

    public async Task<Result> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.SingleOrDefaultAsync(
            x => x.Id == request.UserId,
            cancellationToken);
        user.ConfirmEmail();
        _tenantDbContext.Users.Update(user);
        await _eventStore.AddEvent(new UserEmailConfirmedEvent(user.Id, DateTime.Now));
        await _tenantDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}