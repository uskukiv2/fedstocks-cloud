using gen.fed.application.Abstract;

namespace gen.fed.application.Services;

public interface IUserService : IScopedService
{
    Task<Guid> GetSystemUserIdAsync(int userId);
}