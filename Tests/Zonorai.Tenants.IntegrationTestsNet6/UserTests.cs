using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Add;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.UserClaims.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.ConfirmEmail;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdatePassword;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.UpdateUserDetails;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.Net6WebApi;

namespace Zonorai.Tenants.IntegrationTestsNet6;

public class UserTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _fixture;
    private const string Email = "zonorai@zonorai.com";
    private const string Password = "ZonoraiGmail123#";
    private const string NewPassword = "ZonoraiYahoo123#";

    public UserTests(WebApplicationFactory<Startup> fixture)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        var customConfig = fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) => { conf.AddJsonFile(configPath); });
        });
        var scope = customConfig.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<TenantDbContext>().Database.Migrate();
        _fixture = customConfig;
    }

    private async Task<HttpClient> Authenticate()
    {
        var client = _fixture.CreateClient();
        var result = await Shared.Login(client,Email, Password);
        if (string.IsNullOrEmpty(result.ErrorMessage) == false)
        {
            var tenant = await Shared.RegisterTenant(client, Email, "GSat", Password);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tenant.Token);
            await Shared.CreateClaim(client, "Testing", ClaimTypes.Role);
            await Shared.CreateClaim(client, "ATest", ClaimTypes.Role);
            return client;
        }
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, result.Token);

        return client;
    }
    async Task<UserDto> GetUser(string email = null)
    {
        var client = await Authenticate();
        if (email != null)
        {
            return await client.GetFromJsonAsync<UserDto>($"/api/User/GetUserByEmail?email={email}");
        }
        return await client.GetFromJsonAsync<UserDto>($"/api/User/GetUserByEmail?email={Email}");
    }

    async Task<List<UserClaimDto>> ListUserClaims(string id)
    {
        var client = await Authenticate();
        return await client.GetFromJsonAsync<List<UserClaimDto>>($"/api/User/ListUserClaims?userId={id}");
    }

    [Fact]
    public async Task Can_Add_Remove_Users()
    {
        var client = await Authenticate();
        AddUserCommand addUserCommand = new AddUserCommand();
        addUserCommand.Email = "testing@newuser.com";
        addUserCommand.Name = "Hello";
        addUserCommand.Surname = "Hello";
        addUserCommand.Password = "Testing123**";
        addUserCommand.PhoneNumber = string.Empty;

        var addRes = await client.PostAsJsonAsync("/api/User/AddUser", addUserCommand);
        var addResult = await addRes.Content.ReadFromJsonAsync<Result>();
        addResult.IsSuccess.Should().BeTrue();

        var tenantUsers = await client.GetFromJsonAsync<List<UserDto>>("/api/User/ListUsers");
        tenantUsers.Count.Should().Be(2);

        //Can I log in as the new user?
        var loginResult = await Shared.Login(client, addUserCommand.Email, addUserCommand.Password);
        loginResult.Token.Should().NotBeNullOrEmpty();

        var newUser = await GetUser("testing@newuser.com");
        DeleteUserCommand deleteUserCommand = new DeleteUserCommand();
        deleteUserCommand.UserId = newUser.Id;
        var removeRes = await client.PostAsJsonAsync("/api/User/DeleteUser", deleteUserCommand);
        var removeResult = await removeRes.Content.ReadFromJsonAsync<Result>();

        removeResult.IsSuccess.Should().BeTrue();
        var updatedTenantUsers = await client.GetFromJsonAsync<List<UserDto>>("/api/User/ListUsers");
        updatedTenantUsers.Count.Should().Be(1);
    }

    [Fact]
    public async Task Can_Get_User_By_Email()
    {
        var user = await GetUser();
        user.Should().NotBeNull();
        user.Id.Should().NotBeNullOrEmpty();
        user.Name.Should().NotBeNullOrEmpty();
        user.Surname.Should().NotBeNullOrEmpty();
        user.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Can_Get_User_By_Id()
    {
        var client = await Authenticate();
        var emailUser = await GetUser();
        var user = await client.GetFromJsonAsync<UserDto>($"/api/User/GetUserById?id={emailUser.Id}");
        user.Should().NotBeNull();
        user.Id.Should().NotBeNullOrEmpty();
        user.Id.Should().BeEquivalentTo(emailUser.Id);
        user.Name.Should().NotBeNullOrEmpty();
        user.Surname.Should().NotBeNullOrEmpty();
        user.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Can_Assign_Unassign_Claims()
    {
        var client = await Authenticate();
        var emailUser = await GetUser();
        var claims = await Shared.ListClaims(client);
        claims.Count.Should().BeGreaterOrEqualTo(4);
        var userClaims = await ListUserClaims(emailUser.Id);
        userClaims.Should().NotBeEmpty();
        userClaims.Count.Should().BeGreaterOrEqualTo(2);

        claims.Where(x => x.Value == "Admin" || x.Value == "Owner").ToList().ForEach(x =>
        {
            userClaims.Should().ContainEquivalentOf(new UserClaimDto()
            {
                UserId = emailUser.Id,
                ClaimId = x.Id,
                Value = x.Value,
                Type = x.Type
            });
        });
        var testingClaim = claims.SingleOrDefault(x => x.Value == "Testing" && x.Type == ClaimTypes.Role);
        AddClaimToUserCommand addClaimToUserCommand = new AddClaimToUserCommand()
        {
            ClaimId = testingClaim.Id,
            UserId = emailUser.Id
        };
        var assignRes = await client.PostAsJsonAsync("/api/User/AddClaimToUser", addClaimToUserCommand);
        var assignResult = await assignRes.Content.ReadFromJsonAsync<Result>();
        assignResult.IsSuccess.Should().BeTrue();

        var updatedUserClaims = await ListUserClaims(emailUser.Id);
        updatedUserClaims.Should().ContainEquivalentOf(new UserClaimDto()
        {
            UserId = emailUser.Id,
            ClaimId = testingClaim.Id,
            Value = "Testing",
            Type = ClaimTypes.Role
        });
        RemoveClaimFromUserCommand removeClaimFromUserCommand = new RemoveClaimFromUserCommand()
        {
            ClaimId = testingClaim.Id,
            UserId = emailUser.Id
        };
        var removeRes = await client.PostAsJsonAsync("/api/User/RemoveClaimFromUser", removeClaimFromUserCommand);
        var removeResult = await removeRes.Content.ReadFromJsonAsync<Result>();

        removeResult.IsSuccess.Should().BeTrue();

        var claimsPostRemove = await ListUserClaims(emailUser.Id);
        claimsPostRemove.Count.Should().Be(2);
    }

    [Fact]
    public async Task Can_List_UserClaims()
    {
        var emailUser = await GetUser();
        var userClaims = await ListUserClaims(emailUser.Id);
        userClaims.Count.Should().Be(2);
    }

    [Fact]
    public async Task Can_Update_Details()
    {
        var client = await Authenticate();
        var user = await GetUser();

        UpdateUserDetailsCommand userDetailsCommand = new UpdateUserDetailsCommand()
        {
            Email = Email,
            Id = user.Id,
            Name = "Updated",
            Surname = "Updated",
            PhoneNumber = "004 685 9123"
        };

        var res = await client.PostAsJsonAsync("/api/User/UpdateDetails", userDetailsCommand);
        var result = res.Content.ReadFromJsonAsync<Result>();

        var updatedUser = await GetUser();
        updatedUser.Name.Should().Be("Updated");
        updatedUser.Surname.Should().Be("Updated");
        updatedUser.PhoneNumber.Should().Be("004 685 9123");
    }

    [Fact]
    public async Task Can_Confirm_Email()
    {
        var client = await Authenticate();
        var user = await GetUser();
        user.Should().NotBeNull();
        ConfirmUserEmailCommand command = new ConfirmUserEmailCommand();
        command.UserId = user.Id;

        var res = await client.PostAsJsonAsync("/api/User/ConfirmEmail", command);
        var confirmEmailResult = await res.Content.ReadFromJsonAsync<Result>();

        confirmEmailResult.IsSuccess.Should().BeTrue();

        var updatedUser = await GetUser();
        updatedUser.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task Can_Update_Password()
    {
        var unAuthClient = _fixture.CreateClient();
        var result = await Shared.RegisterTenant(unAuthClient, "blah@blah.com", "Company", "1234John**1");
        var client = unAuthClient;
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, result.Token);
        var response = await client.GetFromJsonAsync<UserDto>($"/api/User/GetUserByEmail?email=blah@blah.com");
        response.Should().NotBeNull();

        UpdatePasswordCommand command = new UpdatePasswordCommand();
        command.UserId = response.Id;
        command.NewPassword = NewPassword;
        command.CurrentPassword = "1234John**1";
        var res = await client.PostAsJsonAsync("/api/User/UpdatePassword", command);
        var updatePasswordResult = await res.Content.ReadFromJsonAsync<Result>();

        updatePasswordResult.IsSuccess.Should().BeTrue();

        //Try login with same password
        var login = await Shared.Login(client, "blah@blah.com", "1234John**1");
        login.ErrorMessage.Should().NotBeNullOrEmpty();
        //Login with new password
        var loginWithNewPassword = await Shared.Login(client, "blah@blah.com", NewPassword);
        loginWithNewPassword.Token.Should().NotBeNullOrEmpty();

    }

}