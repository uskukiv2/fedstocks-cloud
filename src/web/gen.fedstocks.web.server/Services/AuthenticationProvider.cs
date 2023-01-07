using gen.fed.application;
using gen.fed.application.Models.Authentication;
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
                Number = int.TryParse(result.User.Identity?.Name, out var number) ? number : 0
            };
        }
    }
}
