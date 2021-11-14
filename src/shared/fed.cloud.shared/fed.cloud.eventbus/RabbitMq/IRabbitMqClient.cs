using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace fed.cloud.eventbus.RabbitMq
{
    public interface IRabbitMqClient : IDisposable
    {
        bool TryConnect();
        IModel CreateChannel();
    }
}
