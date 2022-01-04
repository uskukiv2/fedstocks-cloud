using fed.cloud.product.infrastructure;
using fedstocks.cloud.web.api.Models.Secure;
using Microsoft.AspNetCore.Identity;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class IdentityService : IIdentityService
{
    [Obsolete]
    private IDictionary<string, Guid> _forcedUserIds;
    private readonly IWebHostEnvironment _hostEnvironment;

    public IdentityService(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
        _forcedUserIds = new Dictionary<string, Guid>();
    }
    
    public async Task<IdentityUser> FindAndApplyUserAsync(IdentityAccess access)
    {
        if (_hostEnvironment.IsDevelopment() && string.IsNullOrEmpty(access.AccessToken) &&
            string.IsNullOrEmpty(access.RefreshToken) && !string.IsNullOrEmpty(access.ForcedClientId))
        {
            if (!_forcedUserIds.TryGetValue(access.ForcedClientId, out var userId))
            {
                userId = Guid.NewGuid();
                _forcedUserIds.Add(access.ForcedClientId, userId);
            }

            return new IdentityUser
            {
                UserName = $"client - {userId}",
                Id = userId.ToString()
            };
        }

        return new IdentityUser();
    }

    public Guid GetUserSub(HttpContext context)
    {
        if (!context.Items.TryGetValue("user-id", out var value) || value == null)
        {
            return Guid.Empty;
        }

        return Guid.Parse(value.ToString());
    }
}