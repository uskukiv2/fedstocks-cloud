﻿using fedstocks.cloud.web.api.Models.Configurations;
using System.IdentityModel.Tokens.Jwt;

namespace fedstocks.cloud.web.api.Middleware
{
    public class UserAppendingMiddleware : IMiddleware
    {
        private readonly IdentityConfiguration _configuration;

        public UserAppendingMiddleware(IdentityConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");
            if (string.IsNullOrEmpty(token.Value))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Redirect(_configuration.IdentityUrl);
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token.Value.ToString().Split("Bearer ")[1]) as JwtSecurityToken;
            if (securityToken != null)
            {
                var userClaim = securityToken.Claims.FirstOrDefault(x => x.Type == "id");
                if (userClaim == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Redirect(_configuration.IdentityUrl);
                    return;
                }
                context.Request.Headers.Add("x-userid", userClaim.Value);
                await next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Redirect(_configuration.IdentityUrl);
        }
    }
}
