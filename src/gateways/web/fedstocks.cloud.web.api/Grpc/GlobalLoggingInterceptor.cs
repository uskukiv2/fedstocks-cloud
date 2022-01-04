using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace fedstocks.cloud.web.api.Grpc;

public class GlobalLoggingInterceptor : Interceptor
{
    private readonly ILogger<GlobalLoggingInterceptor> _logger;

    public GlobalLoggingInterceptor(ILogger<GlobalLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogDebug(
            $"{Environment.NewLine}GRPC Request{Environment.NewLine}Method: {context.Method}{Environment.NewLine}Data: {JsonConvert.SerializeObject(request, Formatting.Indented)}");

        var response = base.AsyncUnaryCall(request, context, continuation);

        _logger.LogDebug(
            $"{Environment.NewLine}GRPC Response{Environment.NewLine}Method: {context.Method}{Environment.NewLine}Data: {JsonConvert.SerializeObject(response, Formatting.Indented)}");

        return response;
    }
}