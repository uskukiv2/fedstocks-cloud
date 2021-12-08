using fed.cloud.common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace fed.cloud.product.domain.Abstraction
{
    public interface IUnitOfWork<out TDbConnection> : IUnitOfWork where TDbConnection : DbConnection
    {
        TDbConnection Connection { get; }
    }
}
