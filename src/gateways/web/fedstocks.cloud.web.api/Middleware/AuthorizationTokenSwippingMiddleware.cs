using fedstocks.cloud.web.api.Infrastructure;
using fedstocks.cloud.web.api.Models.Configurations;

namespace fedstocks.cloud.web.api.Middleware
{
    public class AuthorizationTokenSwippingMiddleware : IMiddleware
    {
        private readonly IdentityConfiguration _identity;

        public AuthorizationTokenSwippingMiddleware(IdentityConfiguration identity)
        {
            _identity = identity;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Cookies[$".gen_apps.{_identity.AccessTokenName}"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add(ConstValues.AuthorizationString, $"{ConstValues.BearerStartString} " + token);
            }

            await next(context);
        }
    }
}
