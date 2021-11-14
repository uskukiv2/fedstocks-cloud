using System;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.store.domain.Abstract;
using fed.cloud.store.domain.Factories;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog.Context;

namespace fed.cloud.store.application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;

    public TransactionBehavior(IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWorkFactory.Create<NpgsqlConnection>();
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var response = default(TResponse);
        var eventName = request.GetType().Name;
        try
        {
            await _unitOfWork.BeginAsync();
            try
            {
                using (LogContext.PushProperty("TransactionContext", null))
                {
                    _logger.LogInformation("---- Begin transaction for {eventName} {request}", eventName, request);

                    response = await next();

                    _logger.LogInformation("---- Commit transaction for {evenName} {request}", eventName, request);

                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Internal transaction error happen ");
                await _unitOfWork.RollbackAsync();
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Pipeline error happen {event} {request}", eventName, request);
            throw;
        }
    }
}