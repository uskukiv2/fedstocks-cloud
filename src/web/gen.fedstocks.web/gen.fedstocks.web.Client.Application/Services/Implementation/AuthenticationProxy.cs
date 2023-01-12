using gen.fedstocks.web.Client.Application.Models.Users;

namespace gen.fedstocks.web.Client.Application.Services.Implementation
{
    internal class AuthorizationProxy : IAuthorizationProxy
    {
        public Task<UserDto> GetUserAsync(string number)
        {
            return Task.FromResult(new UserDto());
        }
    }
}
