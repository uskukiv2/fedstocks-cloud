using fedstocks.cloud.web.api.Models.Secure;
using Microsoft.AspNetCore.Identity;

namespace fedstocks.cloud.web.api.Services;

public interface IIdentityService
{
    Guid GetUserSub(HttpContext context);
}