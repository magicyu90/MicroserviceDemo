using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEventsForStandard.Connection
{
    public interface IRabbitMQConnection
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
