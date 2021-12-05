using fed.cloud.common.Infrastructure;
using fed.cloud.eventbus.Base;

namespace fed.cloud.product.host.Infrastructure;

public class EventServiceConfiguration : IEventServiceConfiguration
{
    public EventServiceConfiguration(string brokerName, string defaultConnection,
        string defaultVectorConfig)
    {
        BrokerName = brokerName;
        EventDatabase = new Database(defaultConnection, defaultVectorConfig);
    }

    public string BrokerName { get; }

    public IDatabase EventDatabase { get; }
}