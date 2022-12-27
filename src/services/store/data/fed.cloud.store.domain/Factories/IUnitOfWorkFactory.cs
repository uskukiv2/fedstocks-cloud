using fed.cloud.store.domain.Abstract;
using System.Data.Common;

namespace fed.cloud.store.domain.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
    }
}