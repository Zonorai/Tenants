using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Zonorai.Tenants.Application.Common.Behaviours;

public class ResultValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ResultValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            if (failures.Any())
            {
                var messages = failures.Select(x => x.ErrorMessage);
                Result result = Result.Fail(string.Join(',', messages));
                return result as TResponse;
            }
        }

        return await next();
    }
}