using System;
using Microsoft.IdentityModel.Tokens;

namespace Zonorai.Tenants.Infrastructure.Configuration;

public class TenantInfrastructureConfiguration
{
    public string JWTSecret { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string DbConnection { get; set; }
    public int JwtExpirationInHours { get; set; }
    public TenantsProvider Provider { get; set; } = TenantsProvider.SqlServer;
    public SymmetricSecurityKey SymmetricSecurityKey => new(Convert.FromBase64String(JWTSecret));
    public SigningCredentials SigningCredentials => new(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
}