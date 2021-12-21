using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;
using Zonorai.Tenants.Domain.Users;
using Zonorai.Tenants.Infrastructure.Services;

namespace Zonorai.Tenants.Infrastructure.Persistence;

public abstract class UserMultiTenantDbContext : MultiTenantDbContext
{
    protected readonly IEventStore EventStore;
    protected readonly IMediator Mediator;

    protected UserMultiTenantDbContext(ITenantInfo tenantInfo, IEventStore eventStore, IMediator mediator) :
        base(tenantInfo)
    {
        EventStore = eventStore;
        Mediator = mediator;
    }

    protected UserMultiTenantDbContext(ITenantInfo tenantInfo, DbContextOptions options, IEventStore eventStore,
        IMediator mediator) : base(tenantInfo, options)
    {
        EventStore = eventStore;
        Mediator = mediator;
    }
    protected DbSet<UserClaim> UserClaims { get; set; }
    protected DbSet<User> Users { get; set; }
    protected DbSet<SecurityClaim> Claims { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var casted = (EventStore) EventStore;
        int result;
        try
        {
            result = await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            casted.Clear();
            throw e;
        }

        foreach (var @event in EventStore.Events) await Mediator.Publish(@event, cancellationToken);

        casted.Clear();
        return result;
    }
}