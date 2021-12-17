using FluentValidation;
using FluentValidation.Results;

namespace Zonorai.Tenants.Domain.Common
{
    public static class Extensions
    {
        public static void ThrowIfFailed(this ValidationResult result)
        {
            if (result.IsValid == false)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}