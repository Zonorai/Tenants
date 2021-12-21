using System.Threading.Tasks;
using FluentResults;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Common;

public interface IUserService
{
    /// <summary>
    ///     Registers a new tenant and creates a Admin/Owner User
    /// </summary>
    /// <param name="registerTenant"></param>
    /// <returns></returns>
    public Task<User> RegisterTenant(RegisterTenant registerTenant);

    /// <summary>
    ///     This method will return a JWT token only if the user belongs to a single tenant else it will return a list of
    ///     tenants to select from for login
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<LoginResult> Login(string email, string password);

    /// <summary>
    ///     Will always return a JWT token provided that credentials are correct and user is not locked
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public Task<LoginResult> Login(string email, string password, string tenantId);

    /// <summary>
    ///     If the user belongs to multiple tenants, then it will only remove the current tenant from it, else a full delete
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<Result> DeleteUser(string userId);

    /// <summary>
    ///     If a user with the provided email already exists this will append the current tenant to it's tenants
    /// </summary>
    /// <param name="createUser"></param>
    /// <returns></returns>
    public Task<Result> AddUser(CreateUser createUser);
}