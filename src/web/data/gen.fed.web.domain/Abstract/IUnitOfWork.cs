using System.Data.Common;
using fed.cloud.common.Infrastructure;

namespace gen.fed.web.domain.Abstract
{
    public interface IUnitOfWork<out TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}