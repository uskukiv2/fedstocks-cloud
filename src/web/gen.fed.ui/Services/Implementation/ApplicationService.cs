using gen.fed.application.Models.Applications;

namespace gen.fed.application.Services.Implementation;

// TODO: FED-23 REDO
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
        //var user = await _authenticationProxy.GetUserAsync(authenticationData.Number);

        return new ApplicationUser
        {
            UserId = 0,
            Username = string.Empty,
            FullName = $"1 2"
        };
    }

    public async Task<int> GetCurrentUserAccountIdAsync()
    {
        return (await _authenticationProvider.GetAuthenticatedUserDataAsync()).Number;
    }
}