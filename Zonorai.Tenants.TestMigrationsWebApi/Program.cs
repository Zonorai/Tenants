using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zonorai.Tenants;
using Zonorai.Tenants.Application;
using Zonorai.Tenants.Application.Claims.Commands.Create;
using Zonorai.Tenants.Application.Users.Commands.Register;
using Zonorai.Tenants.ApplicationInterface;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.Infrastructure;
using Zonorai.Tenants.Infrastructure.Configuration;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddZonoraiMultiTenancy(builder.Configuration,
    tenantApplicationConfiguration: (x) =>
    {
        x.RequireConfirmedEmailForLogin = true;
    });
builder.Services.AddZonoraiMultiTenancy(builder.Configuration,
    tenantInfrastructureConfiguration: (x) =>
    {
        x.JWTSecret = "SomeBase64String";
        x.DbConnection = "SomeSqlConnection";
        x.JwtExpirationInHours = 48;
        x.ValidAudience = "www.somewhere.com";
        x.ValidIssuer = "www.someissues.com";
    });
builder.Services.AddZonoraiMultiTenancy((configuration) =>
    {
        configuration.RequireConfirmedEmailForLogin = true;
    },
    tenantInfrastructureConfiguration: (x) =>
    {
        x.JWTSecret = "SomeBase64String";
        x.DbConnection = "SomeSqlConnection";
        x.JwtExpirationInHours = 48;
        x.ValidAudience = "www.somewhere.com";
        x.ValidIssuer = "www.someissues.com";
    });
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
var context = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
await context.Database.MigrateAsync();
var sender = scope.ServiceProvider.GetRequiredService<IMediator>();
try
{
    var result = await sender.Send(new RegisterCommand()
    {
        Email = "zonorai@zonorai.com",
        Password = "SirZurich1234#*",
        Company = "Testing",
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