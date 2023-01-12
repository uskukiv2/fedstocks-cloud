using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Models.Users;

namespace gen.fedstocks.web.Client.Application.Services
{
    public interface IAuthorizationProxy : IService
    {
        Task<UserDto> GetUserAsync(string number);
    }
}
