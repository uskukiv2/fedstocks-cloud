using gen.fed.ui.Models.Applications;

namespace gen.fed.ui.Services.Implementation;

public class ApplicationService : IApplicationService
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IAuthorizationProxy _authenticationProxy;

    public ApplicationService(IAuthenticationProvider authenticationProvider, IAuthorizationProxy authorizationProxy)
    {
        _authenticationProvider = authenticationProvider;
        _authenticationProxy = authorizationProxy;
    }

    public async Task<ApplicationUser> GetCurrentUserAccountAsync()
    {
        var authenticationData = await _authenticationProvider.GetAuthenticatedUserDataAsync();
        var user = await _authenticationProxy.GetUserAsync(authenticationData.Number);

        return new ApplicationUser
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
}