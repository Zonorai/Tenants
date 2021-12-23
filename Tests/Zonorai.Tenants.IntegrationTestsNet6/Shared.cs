using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentResults;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Create;
using Zonorai.Tenants.ApplicationInterface.Claims.Commands.Delete;
using Zonorai.Tenants.ApplicationInterface.Claims.Queries;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Login;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.Register;
using Zonorai.Tenants.ApplicationInterface.Users.Commands.TryLogin;

namespace Zonorai.Tenants.IntegrationTestsNet6;

public static class Shared
{
    public static async Task<RegisterResult> RegisterTenant(HttpClient client,string email,string name,string password)
    {
        var result = await client.PostAsJsonAsync("/api/User/Register", new RegisterCommand()
        {
            Email = email,
            Password = password,
            Company = name,
            Name = "John",
            Surname = "Doe",
            Website = "www.something.com",
            PhoneNumber = ""
        });
        return await result.Content.ReadFromJsonAsync<RegisterResult>();
    }

    public static async Task<LoginResult> Login(HttpClient client,string email,string password)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/User/Login", new TryLoginCommand()
        {
            Email = email,
            Password = password
        });
        return await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
    }
    public static async Task<LoginResult> LoginToTenant(HttpClient client,string email,string password,string tenantId)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/User/LoginToTenantDirectory", new LoginCommand()
        {
            Email = email,
            Password = password,
            TenantId = tenantId
        });
        return await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
    }
    public static async Task<Result> CreateClaim(HttpClient client,string value,string type)
    {
        var res = await client.PostAsJsonAsync("/api/Claims/Create", new CreateClaimCommand()
        {
            Type = type,
            Value = value
        });
        return await res.Content.ReadFromJsonAsync<Result>();
    }
    public static async Task<Result> DeleteClaim(HttpClient client,string claimId)
    {
        var res = await client.PostAsJsonAsync("/api/Claims/Delete", new DeleteClaimCommand()
        {
            Id = claimId
        });
        return await res.Content.ReadFromJsonAsync<Result>();
    }
    public static async Task<List<ClaimDto>> ListClaims(HttpClient client)
    {
        return await client.GetFromJsonAsync<List<ClaimDto>>("/api/Claims/List");
    }
}