using System.Threading.Tasks;

namespace fed.cloud.eventbus.Base
{
    public interface IIntegrationEventHandler
    {

    }

    public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler where T : IntegrationEvent
    {
        Task Handle(T @event);
    }
}
