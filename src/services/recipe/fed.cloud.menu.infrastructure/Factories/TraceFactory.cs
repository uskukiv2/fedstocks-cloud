using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepoDb;
using RepoDb.Interfaces;

namespace fed.cloud.menu.infrastructure.Factories;

public interface ITraceFactory
{
    ITrace Create<T>();
}

public class TraceFactory : ITraceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TraceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public ITrace Create<T>()
    {
        return new MenuRepoDbTrace(_serviceProvider.GetRequiredService<ILogger<T>>());
    }
}

public class MenuRepoDbTrace : ITrace
{
    private readonly ILogger _logger;

    public MenuRepoDbTrace(ILogger logger)
    {
        _logger = logger;
    }

    public void BeforeExecution(CancellableTraceLog log)
    {
        log.Cancel();
    }

    public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
    }

    public async Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = new())
    {
        _logger.LogTrace(
            $"<<{log.StartTime}>> Begin execute statement for session: " +
            $"{log.SessionId} {log.Key}" +
            $"\n\twith query\n]t\t{log.Statement}" +
            $"\n\twith params\n\t\t{JsonConvert.SerializeObject(log.Parameters.Select(x => new { x.ParameterName, x.Value }))}");
    }

    public async Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = new())
    {
        _logger.LogTrace(
            $"<<{DateTime.UtcNow}>> Finished for {log.SessionId} {log.Key} Total: {log.ExecutionTime} " +
            $"\n\tfor query\n\t\t{log.BeforeExecutionLog.Statement}");
    }
}