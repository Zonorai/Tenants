using System.Collections.Generic;
using Zonorai.Tenants.Domain.Tenants;

namespace Zonorai.Tenants.Application.Users.Commands
{
    public record LoginResult(string Token,List<TenantInformation> Directories,string ErrorMessage);
}