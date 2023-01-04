using fed.cloud.common.Infrastructure;

namespace fed.cloud.eventbus.Base
{
    public interface IEventServiceConfiguration
    {
        string BrokerName { get; }
        IDatabase EventDatabase { get; }
    }
}
