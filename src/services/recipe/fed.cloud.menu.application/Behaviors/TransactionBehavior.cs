using fed.cloud.menu.data.Factories;
using fed.cloud.recipe.application.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace fed.cloud.recipe.application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var callerId = Guid.Empty;
        if (request is RequestBase<TResponse> requestBase)
        {
            callerId = requestBase.UserId;
        }
        var response = default(TResponse);
        var eventName = request.GetType().Name;
        var unitOfWork = _unitOfWorkFactory.CreateDefault();
        try
        {
            await unitOfWork.BeginAsync();
            try
            {
                using (LogContext.PushProperty("TransactionContext", null))
                {
                    _logger.LogInformation("---- Begin transaction for {eventName} {request}", eventName, request);

                    response = await next();

                    _logger.LogInformation("---- Commit transaction for {evenName} {request}", eventName, request);

                    if (callerId != Guid.Empty)
                    {
                        await unitOfWork.CommitAsync(callerId);
                    }
                    else
                    {
                        await unitOfWork.CommitAsync();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Internal transaction error happen ");
                await unitOfWork.RollbackAsync();
            }

            return response!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Pipeline error happen {event} {request}", eventName, request);
            throw;
        }
    }
}