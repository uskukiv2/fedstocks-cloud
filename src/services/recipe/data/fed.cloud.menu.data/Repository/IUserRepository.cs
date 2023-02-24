using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;

namespace fed.cloud.menu.data.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByAuthenticationIdAsync(string authenticationId);
}