using gen.fedstocks.web.Client.Application;
using gen.fedstocks.web.Client.Application.Models.Authentication;

namespace gen.fedstocks.web.Client.Services;

public class AuthenticationProvider : IAuthenticationProvider
{
    public Task<AuthenticationData> GetAuthenticatedUserDataAsync()
    {
        return Task.FromResult(new AuthenticationData()
        {
            Number = 9568
        });
    }
}