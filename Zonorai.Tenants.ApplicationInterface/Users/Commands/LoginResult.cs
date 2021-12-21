namespace Zonorai.Tenants.ApplicationInterface.Users.Commands;

public record LoginResult(string Token, List<TenantInformationDto> Directories, string ErrorMessage);