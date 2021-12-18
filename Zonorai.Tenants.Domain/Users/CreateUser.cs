namespace Zonorai.Tenants.Domain.Users
{
    public record CreateUser
    (
        string Email,
        string Name,
        string Surname,
        string Password
    );
}