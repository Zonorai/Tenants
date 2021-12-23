namespace Zonorai.Tenants.ApplicationInterface.Users.Commands;

public record RegisterResult(
    string? UserId,
    string? Token,
    string? ErrorMessage,
    bool EmailConfirmationRequired
);