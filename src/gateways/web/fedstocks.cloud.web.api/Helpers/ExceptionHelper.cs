using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

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

    public static ObjectResult HandleExceptionWithGrpcRedirect(this Exception ex, ILogger logger, HttpContext context)
    {
        if (ex.InnerException?.InnerException is RpcException rex)
        {
            logger.LogWarning(rex, "remote service error");
            switch (rex.StatusCode)
            {
                case StatusCode.Unavailable:
                    return CreateObjectResult(StatusCodes.Status503ServiceUnavailable, context);
                case StatusCode.Unauthenticated:
                    return CreateObjectResult(StatusCodes.Status403Forbidden, context,
                        "authentication error on service side");
            }
        }

        logger.LogError(ex, "internal error");
        return CreateObjectResult(StatusCodes.Status500InternalServerError, context);
    }

    private static ObjectResult CreateObjectResult(int statusCode, HttpContext context,
        string title = "remote service error", string detail = "")
    {
        var problemDetails = context.RequestServices.GetRequiredService<ProblemDetailsFactory>().CreateProblemDetails(
            context,
            statusCode: statusCode,
            title: title,
            type: string.Empty,
            detail: detail,
            instance: null);

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}