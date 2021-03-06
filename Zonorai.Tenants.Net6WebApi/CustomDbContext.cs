using Finbuckle.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Domain.Users;
using Zonorai.Tenants.Infrastructure.Persistence;

namespace Zonorai.Tenants.Net6WebApi;

/// <summary>
///     Testing integrating context into own context
/// </summary>
public class CustomDbContext : UserMultiTenantDbContext
{
    public CustomDbContext(ITenantInfo tenantInfo, IEventStore eventStore, IMediator mediator) : base(tenantInfo,
        eventStore, mediator)
    {
    }

    public CustomDbContext(ITenantInfo tenantInfo, DbContextOptions options, IEventStore eventStore, IMediator mediator)
        : base(tenantInfo, options, eventStore, mediator)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Something>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
        builder.Entity<Something>().HasKey(x => x.Id);
        base.OnModelCreating(builder);
    }
}

public class Something
{
    public string Id { get; set; }
    public string UserId { get; set; }
}