using gen.fed.ui.Abstract;
using gen.fed.ui.Models.Users;

namespace gen.fed.ui.Services
{
    public interface IAuthorizationProxy : IService
    {
        Task<UserDto> GetUserAsync(string number);
    }
}
