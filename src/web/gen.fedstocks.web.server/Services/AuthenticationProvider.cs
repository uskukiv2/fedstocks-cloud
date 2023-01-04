
using gen.fed.ui;
using gen.fed.ui.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace gen.fedstocks.web.server.Services
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationProvider(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<AuthenticationData> GetAuthenticatedUserDataAsync()
        {
            var result = await _authenticationStateProvider.GetAuthenticationStateAsync();

            return new AuthenticationData
            {
                Number = result.User.Identity?.Name ?? string.Empty
            };
        }
    }
}
