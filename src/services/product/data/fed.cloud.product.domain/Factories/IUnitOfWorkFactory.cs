using System.Data.Common;
using fed.cloud.product.domain.Abstraction;

namespace fed.cloud.product.domain.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
    }
}