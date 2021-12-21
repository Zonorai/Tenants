using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Zonorai.Tenants.Application.Common.Behaviours;

public class UnhandledResultExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse : Result, new()
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledResultExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "api Request: Unhandled Exception for Request {Name} {@Request}", requestName,
                request);

            return Result.Fail(ex.Message) as TResponse;
        }
    }
}