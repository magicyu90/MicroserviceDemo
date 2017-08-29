
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Microservice.BuildingBlocks.EventBusRabbitMQ
{
    public class DefaultRabbitMQPersisterConnection : IRabbitMQPersistentConnection
    {

        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersisterConnection> _logger;

        IConnection _connection;
        bool _disposed;

        object sync_root = new object();

        public DefaultRabbitMQPersisterConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersisterConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;


        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                _connection = _connectionFactory.CreateConnection();

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                    _connection.ConnectionBlocked += _connection_ConnectionBlocked;
                    _connection.CallbackException += _connection_CallbackException;
                    return true;
                }
                else
                {
                    _logger.LogCritical("RabbitMQ connection could not be created and opened");

                    return false;
                }
            }
        }

        private void _connection_CallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ conenction throw exception. Tring to re-connect...");

            TryConnect();
        }

        private void _connection_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Tring to re-connect...");

            TryConnect();
        }


        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
