using System.Collections.Generic;
using Zonorai.Tenants.Entities;

namespace Zonorai.Tenants.Users
{
    public record LoginResult(string Token,List<TenantInformation> Directories,string ErrorMessage);
}