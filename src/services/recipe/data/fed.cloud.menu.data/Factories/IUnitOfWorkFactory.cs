using System.Data.Common;
using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
        IUnitOfWork CreateDefault();
    }
}