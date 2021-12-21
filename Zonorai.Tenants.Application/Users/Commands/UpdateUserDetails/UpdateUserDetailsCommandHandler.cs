using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdateUserDetails;

namespace Zonorai.Tenants.Application.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, Result>
{
    private readonly IEventStore _eventStore;
    private readonly ITenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public UpdateUserDetailsCommandHandler(ITenantDbContext tenantDbContext, ITenantInfo tenantInfo,
        IEventStore eventStore)
    {
        _tenantDbContext = tenantDbContext;
        _tenantInfo = tenantInfo;
        _eventStore = eventStore;
    }

    public async Task<Result> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var user = await _tenantDbContext.Users.Include(x => x.Tenants)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (user.Tenants.Any(x => x.Id == _tenantInfo.Id) == false)
            return Result.Fail("You don't have permissions to edit this user");

        if (string.IsNullOrEmpty(request.PhoneNumber) == false)
            if (user.PhoneNumber != request.PhoneNumber)
                user.SetPhoneNumber(request.PhoneNumber);

        if (user.Email != request.Email) user.SetEmail(request.Email);

        if (user.Name != request.Name) user.SetName(request.Name);

        if (user.Surname != request.Surname) user.SetSurname(request.Surname);

        _tenantDbContext.Users.Update(user);

        await _eventStore.AddEvent(new UserDetailsUpdatedEvent(user.Id, user.Email, user.Surname, user.PhoneNumber,
            user.PhoneNumber, DateTime.Now));

        await _tenantDbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}