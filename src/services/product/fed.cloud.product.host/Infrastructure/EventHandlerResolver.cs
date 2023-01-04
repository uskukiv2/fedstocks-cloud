using fed.cloud.eventbus.Base;

namespace fed.cloud.product.host.Infrastructure;

public class EventHandlerResolver : IHandlerResolver
{
    private readonly IServiceProvider _serviceProvider;

    public EventHandlerResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IIntegrationEventHandler Resolve(string handlerType)
    {
        throw new NotSupportedException("resolver handler by type is not supported yet");
    }

    public IIntegrationEventHandler Resolve(Type handlerType)
    {
        return (IIntegrationEventHandler)_serviceProvider.GetRequiredService(handlerType);
    }
}