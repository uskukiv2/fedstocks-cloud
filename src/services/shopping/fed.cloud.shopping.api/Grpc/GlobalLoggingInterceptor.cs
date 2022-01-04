using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace fed.cloud.shopping.api.Grpc;

public class GlobalLoggingInterceptor : Interceptor
{
    private readonly ILogger<GlobalLoggingInterceptor> _logger;

    public GlobalLoggingInterceptor(ILogger<GlobalLoggingInterceptor> logger)
    {
        _logger = logger;
    }
    
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogDebug(
            $"{Environment.NewLine}GRPC Request{Environment.NewLine}Method: {context.Method}{Environment.NewLine}Data: {JsonConvert.SerializeObject(request, Formatting.Indented)}");

        var response = base.UnaryServerHandler(request, context, continuation);

        _logger.LogDebug(
            $"{Environment.NewLine}GRPC Response{Environment.NewLine}Method: {context.Method}{Environment.NewLine}Data: {JsonConvert.SerializeObject(response, Formatting.Indented)}");

        return response;
    }
}