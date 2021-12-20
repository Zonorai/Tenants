using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;

namespace Zonorai.Tenants.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmUserEmailCommand, Result>
{
    private readonly ITenantDbContext _tenantDbContext;
    private readonly IEventStore _eventStore;
    public ConfirmEmailCommandHandler(ITenantDbContext tenantDbContext, IEventStore eventStore)
    {
        _tenantDbContext = tenantDbContext;
        _eventStore = eventStore;
    }

    public async Task<Result> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x=> x.Tenants).SingleOrDefaultAsync(x => x.Id == request.UserId,
            cancellationToken: cancellationToken);
        user.ConfirmEmail();
        _tenantDbContext.Users.Update(user);
        await _eventStore.AddEvent(new UserEmailConfirmedEvent(user.Id,DateTime.Now));
        await _tenantDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}