using System.Data.Common;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;

namespace fed.cloud.store.domain.Abstract
{
    public interface IUnitOfWork<out TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}