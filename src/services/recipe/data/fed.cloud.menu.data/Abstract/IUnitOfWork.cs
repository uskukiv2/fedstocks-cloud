using System.Data.Common;
using fed.cloud.common.Infrastructure;

namespace fed.cloud.menu.data.Abstract
{
    public interface IUnitOfWork : fed.cloud.common.Infrastructure.IUnitOfWork
    {
        T GetRepository<T>() where T : IRepository;
        
        Task CommitAsync(Guid callerId);
    }
    
    public interface IUnitOfWork<out TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}
