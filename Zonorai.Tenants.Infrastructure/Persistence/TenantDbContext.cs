using System;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Claims;
using Zonorai.Tenants.Domain.Tenants;
using Zonorai.Tenants.Domain.UserClaims;
using Zonorai.Tenants.Domain.Users;
using Zonorai.Tenants.Infrastructure.Services;

namespace Zonorai.Tenants.Infrastructure.Persistence;

public class TenantDbContext : MultiTenantDbContext, ITenantDbContext
{
    private readonly IEventStore _eventStore;
    private readonly IMediator _mediator;
    public TenantDbContext(DbContextOptions<TenantDbContext> options, IMediator mediator, IEventStore eventStore,ITenantInfo tenantInfo) :
        base(tenantInfo,options)
    {
        _mediator = mediator;
        _eventStore = eventStore;
    }

    private TenantDbContext(DbContextOptions<TenantDbContext> options) : base(null,options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<SecurityClaim> Claims { get; set; }
    internal DbSet<TenantInformation> Tenants { get; set; }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var casted = (EventStore) _eventStore;
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

        foreach (var @event in _eventStore.Events) await _mediator.Publish(@event, cancellationToken);

        casted.Clear();
        return result;
    }

    internal static TenantDbContext Create(DbContextOptions<TenantDbContext> options)
    {
        return new TenantDbContext(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}