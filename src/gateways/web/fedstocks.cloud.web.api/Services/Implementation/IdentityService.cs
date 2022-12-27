using fedstocks.cloud.web.api.Infrastructure;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class IdentityService : IIdentityService
{
    public Guid GetUserSub(HttpContext context)
    {
        var value = context.Request.Headers.FirstOrDefault(x => x.Key == ConstValues.HeaderUserIdName);

        return Guid.Parse(value.Value);
    }
}