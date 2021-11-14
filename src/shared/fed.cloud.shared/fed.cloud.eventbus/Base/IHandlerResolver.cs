using System;
using System.Collections.Generic;
using System.Text;

namespace fed.cloud.eventbus.Base
{
    public interface IHandlerResolver
    {
        IIntegrationEventHandler Resolve(string handlerType);
        IIntegrationEventHandler Resolve(Type handlerType);
    }
}
