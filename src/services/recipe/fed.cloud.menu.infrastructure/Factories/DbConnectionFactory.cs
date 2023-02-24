using System.Data;
using System.Data.Common;
using fed.cloud.common.Infrastructure;
using fed.cloud.menu.data.Factories;
using Npgsql;

namespace fed.cloud.menu.infrastructure.Factories;

public class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _defaultConnectionString;

    public NpgsqlDbConnectionFactory(IServiceConfiguration serviceConfiguration)
    {
        _defaultConnectionString = serviceConfiguration.Database.Connection;
    }
    
    public DbConnection CreateDefaultConnection()
    {
        return CreateDbConnection<NpgsqlConnection>(_defaultConnectionString);
    }

    public DbConnection CreateConnection(string connectionString)
    {
        return CreateDbConnection<NpgsqlConnection>(connectionString);
    }

    private DbConnection CreateDbConnection<T>(string connectionString) where T : DbConnection
    {
        return (DbConnection) Activator.CreateInstance(typeof(T), connectionString)!;
    }
}