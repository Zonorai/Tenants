using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Zonorai.Tenants.Application.Common;
using Zonorai.Tenants.Infrastructure.Configuration;

namespace Zonorai.Tenants.Infrastructure.Services
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
        public ClaimsPrincipal ValidateToken(string token)
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
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
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