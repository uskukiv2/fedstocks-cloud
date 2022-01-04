using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

namespace fed.cloud.eventbus.RabbitMq
{
    public class EventBusRabbitMq : IEventBus, IDisposable
    {
        private readonly IEventBusSubscribeManager _subManager;
        private readonly IRabbitMqClient _rabbitMqClient;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly IHandlerResolver _handlerResolver;
        private readonly string _brokerName;

        private IModel _channel;
        private string _queueName;
        private byte _retryCount;

        public EventBusRabbitMq(
            IEventBusSubscribeManager subManager,
            IRabbitMqClient rabbitMqClient,
            IEventServiceConfiguration eventServiceConfiguration,
            IHandlerResolver handlerResolver,
            string queueName,
            ILogger<EventBusRabbitMq> logger)
        {
            _subManager = subManager;
            _rabbitMqClient = rabbitMqClient;
            _queueName = queueName;
            _handlerResolver = handlerResolver;
            _logger = logger;
            _queueName = queueName;
            _brokerName = eventServiceConfiguration.BrokerName;
            _channel = CreateChannel();
            _subManager.OnEventRemoved += _subManager_OnEventRemoved;
            _retryCount = 5;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _subManager.Clear();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_rabbitMqClient.TryConnect())
            {
                return;
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, ra => TimeSpan.FromSeconds(Math.Pow(2, ra)), (ex, time) =>
                {
                    _logger.LogWarning(ex, $"Event {@event.Id} cannot be published total elapsed {time.TotalSeconds:n1} {ex.Message}");
                });

            var eventName = @event.GetType().Name;

            _logger.LogTrace($"Creating RabbitMQ channel to publish event: {@event.Id} ({eventName})");

            using var channel = _rabbitMqClient.CreateChannel();
            _logger.LogTrace($"Declaring RabbitMQ exchange to publish event: {@event.Id}");

            channel.ExchangeDeclare(exchange: _brokerName, type: "direct");

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, Formatting.Indented));

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                _logger.LogTrace($"Publishing event to RabbitMQ: {@event.Id}");

                channel.BasicPublish(
                    exchange: _brokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventName<T>();
            DoInternalSubscription(eventName);

            _logger.LogTrace($"Added subscribtion on event {eventName} with handler {typeof(TH).Name}");

            _subManager.AddSubscription<T, TH>();

            StartBasicConsume();
        }

        public void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subManager.GetEventName<T>();

            _logger.LogTrace($"Request to delete subscribtion on event {eventName} with handler {typeof(TH).Name}");

            _subManager.RemoveSubscribtion<T, TH>();
        }

        private void DoInternalSubscription(string eventName)
        {
            var hasSubs = _subManager.AnySubscriptionForEvent(eventName);
            if (!hasSubs)
            {
                if (!_rabbitMqClient.TryConnect())
                {
                    return;
                }

                _channel.QueueBind(queue: _queueName,
                                    exchange: _brokerName,
                                    routingKey: eventName);
            }
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_channel is null)
            {
                _logger.LogWarning("Cannot continue work with consume");
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }

        private IModel CreateChannel()
        {
            if (!_rabbitMqClient.TryConnect())
            {
                throw new Exception("client is not connected");
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _rabbitMqClient.CreateChannel();

            channel.ExchangeDeclare(exchange: _brokerName,
                                    type: "direct");

            channel.QueueDeclare(queue: _queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _channel.Dispose();
                _channel = CreateChannel();
                StartBasicConsume();
            };

            return channel;
        }

        #region consumer_handling

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            var message = Encoding.UTF8.GetString(@event.Body.Span.ToArray());

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "==== ERROR Processing message \"{Message}\" ====", message);
            }

            // DLX handing should be configured on the server
            _channel.BasicAck(@event.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace($"Processing rabbitMQ event {eventName}");
            if (!_subManager.HasEventHandling(eventName))
            {
                _logger.LogWarning($"no event handing set for {eventName}");
                return;
            }

            var subscribtions = _subManager.GetHandlers(eventName);
            foreach (var subscribtion in subscribtions)
            {
                var handler = _handlerResolver.Resolve(subscribtion.HandlerType);
                if (handler is null)
                {
                    continue;
                }

                var eventType = _subManager.GetEventTypeByName(eventName);
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await Task.Yield();
                await ((Task)concreteType.GetMethod("Handle")?.Invoke(handler, new object[] { integrationEvent }))!;
            }
        }

        #endregion

        private void _subManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_rabbitMqClient.TryConnect())
            {
                return;
            }

            using (var channel = _rabbitMqClient.CreateChannel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: _brokerName,
                    routingKey: eventName);

                if (_subManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _channel.Close();
                }
            }
        }
    }
}
