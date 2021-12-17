using FluentValidation;

namespace Zonorai.Tenants.Domain.Users
{
    public class RegisterTenantValidator : AbstractValidator<RegisterTenant>
    {
        public RegisterTenantValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().NotNull();
            RuleFor(x => x.Surname).NotEmpty().NotNull();
            RuleFor(x => x.Password).MinimumLength(8);
            RuleFor(x => x.CompanyName).NotEmpty().NotNull();
        }
    }
}