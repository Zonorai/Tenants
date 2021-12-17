using Finbuckle.MultiTenant;

namespace Zonorai.Tenants.Domain.Tenants
{
    public class TenantInformation : ITenantInfo
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Website { get; set; }
    }
}