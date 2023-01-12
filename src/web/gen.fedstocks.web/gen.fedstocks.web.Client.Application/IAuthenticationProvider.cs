using gen.fedstocks.web.Client.Application.Models.Authentication;

namespace gen.fedstocks.web.Client.Application
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationData> GetAuthenticatedUserDataAsync();
    }
}
