using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Zonorai.Tenants.Common
{
    public interface ITokenService
    {
        string GenerateToken(ClaimsIdentity claims);
        SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity claims);
        bool ValidateToken(string token);

    }
}