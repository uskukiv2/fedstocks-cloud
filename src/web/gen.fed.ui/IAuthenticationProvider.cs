using gen.fed.application.Models.Authentication;

namespace gen.fed.application
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationData> GetAuthenticatedUserDataAsync();
    }
}
