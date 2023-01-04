using gen.fed.ui.Models.Users;

namespace gen.fed.ui.Services.Implementation
{
    internal class AuthorizationProxy : IAuthorizationProxy
    {
        public Task<UserDto> GetUserAsync(string number)
        {
            return Task.FromResult(new UserDto());
        }
    }
}
