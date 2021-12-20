using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Zonorai.Tenants.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        //Ignore if this is a result response since the result behaviour will take care of it
        if (typeof(Result).IsAssignableFrom(typeof(TResponse)))
        {
            return await next();
        }
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "api Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}