using fed.cloud.product.domain.Abstraction;
using System.Data.Common;

namespace fed.cloud.product.domain.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
    }
}