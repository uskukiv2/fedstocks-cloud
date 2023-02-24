using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace fed.cloud.menu.api.Interceptors;

public class ServerLoggingInterceptor : Interceptor
{
    private readonly ILogger<ServerLoggingInterceptor> _logger;

    public ServerLoggingInterceptor(ILogger<ServerLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogDebug(
            $">> PROTO << [REQUEST] => {context.Method}{Environment.NewLine}" +
            $"data{Environment.NewLine}\t{JsonConvert.SerializeObject(request, Formatting.Indented)}");
        
        var response = await base.UnaryServerHandler(request, context, continuation);

        _logger.LogDebug($">> PROTO << [RESPONSE] <= {context.Method}{Environment.NewLine}" +
                         $"data{Environment.NewLine}\t{JsonConvert.SerializeObject(response, Formatting.Indented)}");

        return response;
    }
}