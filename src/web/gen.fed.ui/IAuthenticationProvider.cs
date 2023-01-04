using gen.fed.ui.Models.Authentication;

namespace gen.fed.ui
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationData> GetAuthenticatedUserDataAsync();
    }
}
