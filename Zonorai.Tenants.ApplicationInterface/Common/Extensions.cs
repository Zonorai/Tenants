using FluentValidation;
using FluentValidation.Results;

namespace Zonorai.Tenants.ApplicationInterface.Common
{
    public static class Extensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Password must not be empty")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters or longer")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            return options;
        }
    }
}