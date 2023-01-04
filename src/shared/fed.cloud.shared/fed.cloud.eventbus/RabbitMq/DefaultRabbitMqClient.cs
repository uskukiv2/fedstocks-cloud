using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace fed.cloud.eventbus.RabbitMq
{
    public class DefaultRabbitMqClient : IRabbitMqClient
    {
        private static object _lock = new object();

        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMqClient> _logger;
        private readonly int _maxRetry;
        private readonly int _maxTimeout;

        private IConnection _connection;
        private bool _disposed = false;

        public DefaultRabbitMqClient(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMqClient> logger, int maxRetry = 5, int maxTimeout = 1000)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _maxRetry = maxRetry;
            _maxTimeout = maxTimeout;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionBlocked -= OnConnectionBlocked;
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_lock)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_maxRetry, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {
                            _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                        }
                    );

                policy.Execute(() =>
                {
                    _connection = _connectionFactory
                        .CreateConnection();
                });

                if (IsClientConnected())
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }

                _logger.LogCritical("RabbitMQ connections could not be created and opened ");

                return false;
            }
        }

        private bool IsClientConnected() => _connection is { IsOpen: true } && !_disposed;

        public IModel CreateChannel()
        {
            if (!IsClientConnected())
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning($"A RabbitMQ connection is shutdown {e.Reason}");

            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogCritical(e.Exception, $"A RabbitMQ connection throw exception {e.Detail}");

            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown");

            TryConnect();
        }
    }
}