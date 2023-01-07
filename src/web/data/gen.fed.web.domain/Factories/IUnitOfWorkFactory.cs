using System.Data.Common;
using gen.fed.web.domain.Abstract;

namespace gen.fed.web.domain.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection;
    }
}