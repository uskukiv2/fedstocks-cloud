using Grpc.Core;

namespace fedstocks.cloud.web.api.Helpers;

public static class ExceptionHelper
{
    public static T HandleExceptionWithGrpc<T>(ILogger logger, Exception ex) where T : new()
    {
        if (ex.InnerException?.InnerException is RpcException rex)
        {
            logger.LogWarning(rex, "remote service issue");
            switch (rex.StatusCode)
            {
                case StatusCode.Unavailable:
                    return default(T);
                case StatusCode.NotFound:
                case StatusCode.Cancelled:
                case StatusCode.Aborted:
                    return new T();
            }
        }
        
        logger.LogError(ex, "unexpected exception");
        return default(T);
    }
}