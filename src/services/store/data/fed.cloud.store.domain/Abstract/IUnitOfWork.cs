using fed.cloud.common.Infrastructure;
using System.Data.Common;

namespace fed.cloud.store.domain.Abstract
{
    public interface IUnitOfWork<out TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}