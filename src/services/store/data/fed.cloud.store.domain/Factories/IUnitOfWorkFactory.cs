using System.Data.Common;
using System.Runtime.InteropServices;
using fed.cloud.store.domain.Abstract;

namespace fed.cloud.store.domain.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
    }
}