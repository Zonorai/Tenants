using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Zonorai.Tenants.Common;

namespace Zonorai.Tenants
{
    internal class TokenService : ITokenService
    {
        private readonly IOptions<TenantsConfiguration> _config;

        public TokenService(IOptions<TenantsConfiguration> config)
        {
            _config = config;
        }


        public string GenerateToken(ClaimsIdentity claims)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = GetTokenDescriptor(claims);

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config.Value.ValidIssuer,
                    ValidAudience = _config.Value.ValidAudience,
                    IssuerSigningKey = _config.Value.SymmetricSecurityKey
                };
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity claims)
        {
            return new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(2),
                //Issuer = _config.ValidIssuer,
                //Audience = _config.ValidAudience,
                SigningCredentials = _config.Value.SigningCredentials
            };
        }
    }
}