using RabbitMQ.Client;

namespace Microservice.BuildingBlocks.EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
