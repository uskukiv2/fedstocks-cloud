using fedstocks.cloud.web.api.Models.Secure;
using fedstocks.cloud.web.api.Services;
using Microsoft.AspNetCore.Authentication;

namespace fedstocks.cloud.web.api.Middleware;

/// <summary>
/// Should be used only for development with no client
/// </summary>
public class DevelopmentClientAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IIdentityService _service;

    public DevelopmentClientAuthenticationMiddleware(RequestDelegate next, IIdentityService service)
    {
        _next = next;
        _service = service;
    }
    
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("x-client", out var localClientId))
        {
            var identityUser = await _service.FindAndApplyUserAsync(new IdentityAccess
            {
                ForcedClientId = localClientId
            });

            context.Items.TryAdd("user-id", identityUser.Id);
        }

        await _next(context);
    }
}