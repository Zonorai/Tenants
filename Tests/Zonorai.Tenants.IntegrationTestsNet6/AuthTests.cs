using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Add;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;
using Zonorai.Tenants.ApplicationInterface.Users.Queries;
using Zonorai.Tenants.Infrastructure;
using Zonorai.Tenants.Infrastructure.Configuration;
using Zonorai.Tenants.Infrastructure.Persistence;
using Zonorai.Tenants.Net6WebApi;

namespace Zonorai.Tenants.IntegrationTestsNet6;

public class AuthTests : IClassFixture<WebApplicationFactory<Startup>>
{
    readonly HttpClient _client;
    private const string Password = "SirTestCase@3";
    
    public AuthTests(WebApplicationFactory<Startup> fixture)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        var customConfig = fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, conf) => { conf.AddJsonFile(configPath); });
        });
        var scope = customConfig.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<TenantDbContext>().Database.Migrate();
        
        _client = customConfig.CreateClient();
    }


    [Fact]
    public async Task Invalid_Credentials_Fail_Login()
    {
        var result = await _client.PostAsJsonAsync("/api/User/Login", new LoginCommand()
        {
            Email = "notanemail@email.com",
            Password = "ThisIsATest2512**2"
        });
        var loginResult = await result.Content.ReadFromJsonAsync<LoginResult>();
        loginResult.Should().NotBeNull();
        loginResult.Directories.Should().BeNull();
        loginResult.Token.Should().BeNullOrEmpty();
        loginResult.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Authorization_Fails_Without_Token()
    {
        var result = await _client.GetAsync("/api/User/ListUsers");
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Can_Register()
    {
        var result = await RegisterTenant("sir@gmail.com","netflix");
        result.Token.Should().NotBeEmpty();
        result.UserId.Should().NotBeEmpty();
        result.EmailConfirmationRequired.Should().BeFalse();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Can_Login_With_Single_Tenant()
    {
        var result = await RegisterTenant("madam@gmail.com","ms");
        var loginResult = await Shared.Login(_client, "madam@gmail.com", Password);
        loginResult.ErrorMessage.Should().BeNullOrEmpty();
        loginResult.Directories.Should().BeNullOrEmpty();
        loginResult.Token.Should().NotBeEmpty().Should().NotBeNull();
    }
    [Fact]
    public async Task Can_Login_With_Multiple_Tenants()
    {
        var result = await RegisterTenant("john@gmail.com","google");
        var secondTenant = await RegisterTenant("doe@gmail.com","yahoo");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, secondTenant.Token);
        var addResponse = await _client.PostAsJsonAsync("/api/User/AddUser", new AddUserCommand()
        {
            Email = "john@gmail.com",
            Password = "PasswordToShowTheUserJustGetsAddedToTenantNotModified122**",
            Name = "123",
            Surname = "123",
            PhoneNumber = ""
        });
        
        var addResult = await addResponse.Content.ReadFromJsonAsync<Result>();
        addResult.IsSuccess.Should().BeTrue();
        
        var loginResult = await Shared.Login(_client,"john@gmail.com",Password);
        loginResult.ErrorMessage.Should().BeNullOrEmpty();
        loginResult.Directories.Should().HaveCount(2);
        loginResult.Token.Should().BeNullOrEmpty();
        
        var tenantLoginResult = await Shared.LoginToTenant(_client,"john@gmail.com",Password,loginResult.Directories.First().Id);
        
        tenantLoginResult.ErrorMessage.Should().BeNullOrEmpty();
        tenantLoginResult.Directories.Should().BeNullOrEmpty();
        tenantLoginResult.Token.Should().NotBeNullOrEmpty();
    }
    private async Task<RegisterResult> RegisterTenant(string email,string name)
    {
        return await Shared.RegisterTenant(_client, email, name, Password);
    }
}