using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

//THANK YOU @ https://mudblazor.com/components/form#using-simple-fluent-validation
namespace Zonorai.Tenants.Common
{
    /// <summary>
    /// A glue class to make it easy to define validation rules for single values using FluentValidation
    /// You can reuse this class for all your fields, like for the credit card rules above.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FluentValueValidator<T> : AbstractValidator<T>
    {
        private FluentValueValidator(Action<IRuleBuilderInitial<T, T>> rule)
        {
            rule(RuleFor(x => x));
        }

        internal static void Validate(T value, Action<IRuleBuilderInitial<T, T>> rules)
        {
            var validator = new FluentValueValidator<T>(rules);
            var isValid = validator.Validate(value);
            isValid.ThrowIfFailed();
        }
        private IEnumerable<string> ValidateValue(T arg)
        {
            var result = Validate(arg);
            if (result.IsValid)
                return new string[0];
            return result.Errors.Select(e => e.ErrorMessage);
        }

        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}