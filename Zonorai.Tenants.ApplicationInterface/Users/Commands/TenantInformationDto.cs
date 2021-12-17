namespace Zonorai.Tenants.ApplicationInterface.Users.Commands;

public class TenantInformationDto
{
    public string Id { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string Website { get; set; }
}