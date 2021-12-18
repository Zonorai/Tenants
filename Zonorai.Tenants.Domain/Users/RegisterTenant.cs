namespace Zonorai.Tenants.Domain.Users
{
    public record RegisterTenant
    (
        string CompanyName,
        string CompanyWebsite,
        string Email,
        string Name,
        string Surname,
        string Password
    );
}