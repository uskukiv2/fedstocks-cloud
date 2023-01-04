using RabbitMQ.Client;
using System;

namespace fed.cloud.eventbus.RabbitMq
{
    public interface IRabbitMqClient : IDisposable
    {
        bool TryConnect();
        IModel CreateChannel();
    }
}
