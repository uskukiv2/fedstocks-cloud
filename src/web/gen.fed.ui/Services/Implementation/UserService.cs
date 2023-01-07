using gen.fed.web.domain.Repositories;

namespace gen.fed.application.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Guid> GetSystemUserIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByInternalId(userId);

        return user.Id;
    }
}