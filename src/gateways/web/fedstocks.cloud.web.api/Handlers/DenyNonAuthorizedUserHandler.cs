using Microsoft.AspNetCore.Authorization;

namespace fedstocks.cloud.web.api.Handlers
{
    public class DenyNonAuthorizedUserRequirement : AuthorizationHandler<DenyNonAuthorizedUserRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyNonAuthorizedUserRequirement requirement)
        {
            return Task.CompletedTask;
        }
    }
}
