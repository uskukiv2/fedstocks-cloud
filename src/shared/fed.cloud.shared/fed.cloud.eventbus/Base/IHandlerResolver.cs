using System;

namespace fed.cloud.eventbus.Base
{
    public interface IHandlerResolver
    {
        IIntegrationEventHandler Resolve(string handlerType);
        IIntegrationEventHandler Resolve(Type handlerType);
    }
}
