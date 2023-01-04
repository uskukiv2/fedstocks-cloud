using fed.cloud.store.application.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IValidator<TRequest>[] _validators;
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;

    public ValidatorBehavior(IValidator<TRequest>[] validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var typeName = request.GetType().Name;

        _logger.LogInformation("COMMAND HANDLING ----- Validating command {CommandType}", typeName);

        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (!failures.Any())
        {
            return await next();
        }

        _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

        throw new StoreDomainException(
            $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));

    }
}