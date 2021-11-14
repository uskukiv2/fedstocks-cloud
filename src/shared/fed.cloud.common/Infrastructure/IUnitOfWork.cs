using System.Data.Common;
using System.Threading.Tasks;

namespace fed.cloud.common.Infrastructure
{
    public interface IUnitOfWork
    {
        DbTransaction? Transaction { get; }

        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
