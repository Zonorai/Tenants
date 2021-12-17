namespace Zonorai.Tenants.Users
{
    public record RegisterTenant
    (
        string CompanyName,
        string CompanyWebsite,
        string Email,
        string Username,
        string Name,
        string Surname,
        string Password
    );
}