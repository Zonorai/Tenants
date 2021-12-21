
# Zonorai.Tenants :rocket:
# Powered By [![N|Solid](https://www.finbuckle.com/images/favicon-32x32.png)](https://www.finbuckle.com/)

This repository integrates with Finbuckle.MultiTenant using a Claim-Based Strategy and provides common use-cases that go along with multi-tenancy.

## Implementation Details
Functions are exposed as **Mediatr Command/Query Endpoints** with **Notifications** for each command, making it easy to **extend** this library.

Persistence is driven by **EF CORE**.

**Clean-Architecture** was used to build out this project and each respective layer has a separate nuget package.

The **Application Interface Layer** is for sharing requests and validators with a C# client (Blazor/Xamarin etc).

## User Features
- Registration
- Login (JWT)
- Update/Reset Passwords
- Update User Details
- Email Confirmation
- Add to Tenant
- Remove from Tenant
- Assign/Unassign Claims (Is MultiTenant)

## Claim Features
- Add/Remove (Is MultiTenant)

## General Features
- Setup for JWT Authentication/Authorization provided
- ***UserMultiTenantDbContext*** extends MultiTenantDbContext and makes it possible to create a context with relationships to users/claims.
- Behaviours included for handling exceptions
- Behaviours for Validation
- Validation/Exception Behaviours specific for requests that return a **Result** object.

# Setup

### Example appsettings.json Sections

    "TenantApplicationConfiguration": 
    {
        "RequireConfirmedEmailForLogin": false
    },
    "TenantInfrastructureConfiguration": 
    {
        "JWTSecret": "B83FD32C00E12636813581E8FE8F889398B37CEA3B2CB7F32C05787EAF831335",
        "ValidIssuer": "www.sometissuer.com",
        "ValidAudience": "www.something.com",
        "DbConnection": "SomeSqlConnection",
        "JwtExpirationInHours": 48
    },


#### Add Services Using AppSettings.json
`services.AddZonoraiMultiTenancy(Configuration);`

#### Manual Configuration Overloads also available

    services.AddZonoraiMultiTenancy
    (Configuration,
        tenantApplicationConfiguration: 
        (x) =>
        {
            x.RequireConfirmedEmailForLogin = true;
        }
    );

    services.AddZonoraiMultiTenancy(Configuration,
        tenantInfrastructureConfiguration: (x) =>
       {
            x.JWTSecret = "SomeBase64String";
            x.DbConnection = "SomeSqlConnection";
            x.JwtExpirationInHours = 48;
            x.ValidAudience = "www.somewhere.com";
            x.ValidIssuer = "www.someissuer.com";
        });
    
        services.AddZonoraiMultiTenancy(
        (appConfiguration) =>  
        {  
		     appConfiguration.RequireConfirmedEmailForLogin = true;  
	    },  
	     (x) =>  
        {  
		     x.JWTSecret = "SomeBase64String";  
		     x.DbConnection = "SomeSqlConnection";  
		     x.JwtExpirationInHours = 48;  
		     x.ValidAudience = "www.somewhere.com";  
		     x.ValidIssuer = "www.someissues.com";  
        }
     );

### Required Middlewares

`app.UseMultiTenant();`

`app.UseAuthentication();`

`app.UseAuthorization();`

## Examples
**Registering a new tenant from a controller endpoint**

    [ApiController]
    public class UserController : ControllerBase
    { 
        private readonly ISender _sender;
            
        public UserController(ISender sender)
        {
	        _sender = sender
        }
            
        [HttpPost]
        [Route("[action]")]
        public async Task<RegisterResult> Register([FromBody] RegisterCommand command)
        {
          var result = await _sender.Send(command);
          return result;
        }
    }


**Reacting to registration with a confirmation email**

    public class RegistrationEventHandler : INotificationHandler<TenantRegisteredEvent>  
    {  
          
      public Task Handle(TenantRegisteredEvent notification, CancellationToken cancellationToken)  
      {  
	     ...Send an email here then direct it to an endpoint that invokes the Confirm
	      Email Command
       }
    }
**Creating your own DbContext based on the entities provided by this library**

    public class UserBookmarks
    {
		public string Id { get; set; }
		public string UserId { get; set; }
		public string Url { get; set; }
		public string TenantId { get; set; }
    }

    public class CustomDbContext : UserMultiTenantDbContext  
    {  

      public CustomDbContext
      (
      ITenantInfo tenantInfo, 
      IEventStore eventStore, IMediator mediator
      ) : base(tenantInfo, eventStore, mediator)  
      { 
      
      }  
     
      public CustomDbContext
      (
	      ITenantInfo tenantInfo, 
	      DbContextOptions options, IEventStore eventStore,
	      IMediator mediator) : base(tenantInfo, options, eventStore, mediator
	  )  
      { 
      
      }
      
      protected override void OnModelCreating(ModelBuilder builder)  
      { 
	      builder.Entity<UserBookmarks>().HasOne<User>().WithMany().HasForeignKey(x => x.UserId);  
	      builder.Entity<UserBookmarks>().HasKey(x => x.Id);  
	      builder.Entity<UserBookmarks>().IsMultiTenant();
	      base.OnModelCreating(builder);  
      }  
    }
**If you only want to use the existing TenantDbContext then you can call migrate on the context, existing migrations are provided**



    var app = builder.Build();  
    app.UseMultiTenant();  
    app.UseAuthentication();  
    app.UseAuthorization();  
    var scope = app.Services.CreateScope();  
    var context = scope.ServiceProvider.GetRequiredService<TenantDbContext>();  
    await context.Database.MigrateAsync();

**All commands have a notification that gets published once the changes are persisted**
*This allows you to react to every action that occurs in this library effectively allowing all the extension you need.*

*The publishing occurs in the DbContext SaveChangesAsync method and if you inherit from UserMultiTenantDbContext you can also publish all your events post persistence.*

*To achieve this make use of the IEventStore interface and add your INotification object prior to calling SaveChangesAsync*

**Example**


      public class SomeHandler : IRequestHandler<SomeRequest, Result>  
      {   
          private readonly IEventStore _eventStore; 
          private readonly TheDbContext _context;
          
          public SomeHandler(IEventStore eventStore, TheDbContext context)  
          {   
	          _eventStore = eventStore;
	          _context = context;  
          }  
          
          public async Task<Result> Handle(SomeRequest request, CancellationToken cancellationToken)  
          {  
         
	          await _eventStore.AddEvent(new SomeEvent());  
              await _context.SaveChangesAsync(cancellationToken);  
	          return Result.Ok();  
          }
      }

## Commands/Notifications Table exposed by the library

| Command |Notification  |
|--|--|
| CreateClaimCommand | ClaimCreatedEvent |
| DeleteClaimCommand | ClaimDeletedEvent |
| AddClaimToUserCommand | ClaimAddedToUserEvent|
| RemoveClaimFromUserCommand | ClaimRemovedFromUserEvent|
| AddUserCommand | UserAddedEvent |
| ConfirmUserEmailCommand | UserEmailConfirmedEvent |
| DeleteUserCommand | UserDeletedEvent |
| LoginCommand | UserLoggedInEvent |
| RegisterCommand | TenantRegisteredEvent |
| ResetPasswordCommand | PasswordResetCommand|
| TryLoginCommand | N/A|
| UpdatePasswordCommand | PasswordUpdatedEvent|
| UpdateUserDetailsCommand | UserDetailsUpdatedEvent|

## List of queries exposed by the library

 - GetUserByEmailQuery
 - GetUserByIdQuery
 - ListUsersQuery
 - ListUserClaimsCommand
 - ListClaimsQuery

## License

MIT

**Hope this saves you some time and effort!**
***
**With <3 Zonorai**
