namespace Zonorai.Tenants.Users
{
    public record CreateUser
    (
        string Email,
        string Username,
        string Name,
        string Surname,
        string Password
    );
}