using System.Security.Claims;

namespace Zonorai.Tenants.Application.Common;

public interface ITokenService
{
    string GenerateToken(ClaimsIdentity claims);
    ClaimsPrincipal ValidateToken(string token);
}