using fedstocks.cloud.web.api.Models.Secure;
using Microsoft.AspNetCore.Identity;

namespace fedstocks.cloud.web.api.Services;

public interface IIdentityService
{
    Task<IdentityUser> FindAndApplyUserAsync(IdentityAccess access);
    Guid GetUserSub(HttpContext context);
}