namespace fed.cloud.common.Infrastructure
{
    public interface IDatabase
    {
        string Connection { get; }
        string VectorConfig { get; }
    }

    public class Database : IDatabase
    {
        public Database(string connectionString, string vectorConfig)
        {
            Connection = connectionString;
            VectorConfig = vectorConfig;
        }

        public string Connection { get; }

        public string VectorConfig { get; }
    }
}