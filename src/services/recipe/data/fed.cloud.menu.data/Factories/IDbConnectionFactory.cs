using System.Data;
using System.Data.Common;

namespace fed.cloud.menu.data.Factories;

public interface IDbConnectionFactory
{
    DbConnection CreateDefaultConnection();

    DbConnection CreateConnection(string connectionString);
}