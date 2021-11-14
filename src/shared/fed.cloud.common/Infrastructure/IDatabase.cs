namespace fed.cloud.common.Infrastructure
{
    public interface IDatabase
    {
        string Connection { get; }
    }

    public class Database : IDatabase
    {
        public Database(string connectionString)
        {
            Connection = connectionString;
        }

        public string Connection { get; }
    }
}