using System.Threading.Tasks;
using Zonorai.Tenants.Application.Users.Commands;
using Zonorai.Tenants.ApplicationInterface.Users.Commands;
using Zonorai.Tenants.Domain.Users;

namespace Zonorai.Tenants.Application.Common
{
    public interface IUserService
    {
        public Task<User> RegisterTenant(RegisterTenant registerTenant);
        public Task<LoginResult> Login(string email, string password);
        public Task<LoginResult> Login(string email, string password, string tenantId);
    }
}