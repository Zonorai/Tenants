using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zonorai.Tenants;
using Zonorai.Tenants.Application.Claims.Commands.Create;
using Zonorai.Tenants.Application.Users.Commands.Register;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.Infrastructure.Configuration;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddZonoraiMultiTenancy(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanPurge2", policy => policy.RequireRole("Owner2"));
});
builder.Services.AddDbContext<CustomDbContext>(x =>
    x.UseSqlServer(
        builder.Configuration.GetSection(nameof(TenantInfrastructureConfiguration))
            .GetValue<string>(nameof(TenantInfrastructureConfiguration.DbConnection)),
        y => y.MigrationsAssembly(typeof(Program).Assembly.FullName)));
var app = builder.Build();
app.UseMultiTenant();
app.UseAuthentication();
app.UseAuthentication();
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
await context.Database.MigrateAsync();
var sender = scope.ServiceProvider.GetRequiredService<IMediator>();
try
{
    var result = await sender.Send(new RegisterCommand
    {
        Email = "zonorai@zonorai22.com",
        Password = "SirZurich1234#*",
        Company = "Testing22",
        Name = "John",
        Surname = "Doe",
        Website = "www.gooogle.com"
    });
    var x = result;
    var claimResult = await sender.Send(new CreateClaimCommand
    {
        Type = "Hello",
        Value = "Hello"
    });
    var y = claimResult;
}
catch (Exception e)
{
    var x = e;
}


app.MapGet("/", () => "Hello World!");

app.Run();


public class Registration2Listener : INotificationHandler<ClaimCreatedEvent>
{
    private readonly ILogger<Registration2Listener> _logger;

    public Registration2Listener(ILogger<Registration2Listener> logger)
    {
        _logger = logger;
    }


    public Task Handle(ClaimCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Claim Created");
        var x = notification;
        return Task.CompletedTask;
    }
}

public class RegistrationListener : INotificationHandler<TenantRegisteredEvent>
{
    private readonly ILogger<RegistrationListener> _logger;

    public RegistrationListener(ILogger<RegistrationListener> logger)
    {
        _logger = logger;
    }

    public Task Handle(TenantRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User Registered");
        var x = notification;
        return Task.CompletedTask;
    }
}