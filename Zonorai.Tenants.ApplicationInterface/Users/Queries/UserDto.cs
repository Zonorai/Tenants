namespace Zonorai.Tenants.ApplicationInterface.Users.Queries;

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public bool EmailConfirmed { get; set; }
    public string FullName => $"{Name} {Surname}";
}