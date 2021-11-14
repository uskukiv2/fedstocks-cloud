using fed.cloud.common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace fed.cloud.eventbus.Base
{
    public interface IEventServiceConfiguration
    {
        string BrokerName { get; }
        IDatabase EventDatabase { get; }
    }
}
