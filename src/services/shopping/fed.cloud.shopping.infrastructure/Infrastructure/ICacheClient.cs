using StackExchange.Redis;

namespace fed.cloud.shopping.infrastructure.Infrastructure;

public interface ICacheClient
{
    IConnectionMultiplexer Connection { get; }

    IDatabase Database { get; }
}

public class CacheClient : ICacheClient
{
    private readonly Lazy<IConnectionMultiplexer> _connection;

    public CacheClient(string host, int port, string username, string password, string clientName)
    {
        var configuration = new ConfigurationOptions()
        {
            EndPoints = { { host, port }, },
            User = username,
            Password = password,
            ClientName = clientName,
            ReconnectRetryPolicy = new LinearRetry(5000),
            AbortOnConnectFail = false,
        };

        _connection = new Lazy<IConnectionMultiplexer>(() =>
        {
            var redis = ConnectionMultiplexer.Connect(configuration);
            return redis;
        });
    }

    public IConnectionMultiplexer Connection => _connection.Value;

    public IDatabase Database => Connection.GetDatabase();
}