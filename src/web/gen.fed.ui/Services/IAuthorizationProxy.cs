using gen.fed.application.Abstract;
using gen.fed.application.Models.Users;

namespace gen.fed.application.Services
{
    public interface IAuthorizationProxy : IService
    {
        Task<UserDto> GetUserAsync(string number);
    }
}
