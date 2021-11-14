namespace fed.cloud.common.Infrastructure
{
    public interface IServiceConfiguration
    {
        IDatabase Database { get; }

        string GetSchema();

        DatabaseType GetActiveDatabaseType();
    }

    public class ServiceConfiguration : IServiceConfiguration
    {
        private readonly DatabaseType _defaultType;
        private readonly string _defaultSchema;

        public ServiceConfiguration(IDatabase database, DatabaseType type, string schema)
        {
            Database = database;
            _defaultType = type;
            _defaultSchema = schema;
        }

        public IDatabase Database { get; }

        public DatabaseType GetActiveDatabaseType() => _defaultType;

        public string GetSchema() => _defaultSchema;
    }
}
