using System;
using Microsoft.IdentityModel.Tokens;

namespace Zonorai.Tenants.Common
{
    public class TenantsConfiguration
    {
        public string JWTSecret { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string DbConnection { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Convert.FromBase64String(JWTSecret));
        public SigningCredentials SigningCredentials => new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }
}