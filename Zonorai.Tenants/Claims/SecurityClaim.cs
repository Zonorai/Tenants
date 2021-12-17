using System;
using System.Security.Claims;
using FluentValidation;
using Zonorai.Tenants.Common;

namespace Zonorai.Tenants.Claims
{
    public class SecurityClaim
    {
        public SecurityClaim(string type,string value)
        {
            FluentValueValidator<string>.Validate(type,x=> x.NotEmpty().NotNull());
            FluentValueValidator<string>.Validate(value,x=> x.NotEmpty().NotNull());
            
            Id = Guid.NewGuid().ToString();
            Type = type;
            Value = value;
        }
        public string Id { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }
    }
}