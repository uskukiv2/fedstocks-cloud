using gen.fed.application.Models.Users;

namespace gen.fed.application.Services.Implementation
{
    internal class AuthorizationProxy : IAuthorizationProxy
    {
        public Task<UserDto> GetUserAsync(string number)
        {
            return Task.FromResult(new UserDto());
        }
    }
}
