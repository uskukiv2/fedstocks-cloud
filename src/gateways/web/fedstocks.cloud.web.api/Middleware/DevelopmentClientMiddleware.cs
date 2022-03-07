using fedstocks.cloud.web.api.Models.Secure;
using fedstocks.cloud.web.api.Services;
using Microsoft.AspNetCore.Authentication;

namespace fedstocks.cloud.web.api.Middleware;

/// <summary>
/// Should be used only for development with no client
/// </summary>
public class DevelopmentClientAuthenticationMiddleware : IMiddleware
{
    private readonly IIdentityService _service;

    public DevelopmentClientAuthenticationMiddleware(IIdentityService service)
    {
        _service = service;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("x-client", out var localClientId))
        {
            var identityUser = await _service.FindAndApplyUserAsync(new IdentityAccess
            {
                ForcedClientId = localClientId
            });

            context.Items.TryAdd("user-id", identityUser.Id);
        }

        await next(context);
    }
}