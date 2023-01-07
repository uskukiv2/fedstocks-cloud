using System.Threading.Tasks;
using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Entities;

namespace gen.fed.web.domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByInternalId(int internalId);
    }
}