using fed.cloud.common;
using fed.cloud.common.Infrastructure;

namespace fed.cloud.product.host.Infrastructure;

public class ServiceConfiguration : IServiceConfiguration
{
    private readonly string _defaultSchema;
    private readonly DatabaseType _currentDbType;

    public ServiceConfiguration(string defaultSchema, DatabaseType currentDbType, string defaultConnection,
        string defaultVectorConfig)
    {
        _defaultSchema = defaultSchema;
        _currentDbType = currentDbType;
        Database = new Database(defaultConnection, defaultVectorConfig);
    }

    public IDatabase Database { get; }

    public string GetSchema() => _defaultSchema;

    public DatabaseType GetActiveDatabaseType() => _currentDbType;
}