using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.product.application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("COMMAND HANDLING ----- Handling command {CommandName} ({@Command})", request.GetType().Name, request);
        var response = await next();
        _logger.LogInformation("COMMAND HANDLING ----- Command {CommandName} handled - response: {@Response}", request.GetType().Name, response);

        return response;
    }
}