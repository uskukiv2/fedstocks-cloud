namespace fedstocks.cloud.web.api.Services;

public interface IIdentityService
{
    Guid GetUserSub(HttpContext context);
}