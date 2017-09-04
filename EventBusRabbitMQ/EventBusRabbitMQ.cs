using Autofac;
using Microservice.BuildingBlocks.EventBus;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microservice.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.BuildingBlocks.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "microservice_event_bus";

        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IRabbitMQPersistentConnection _persistConnection;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "microservice_event_bus";

        private IModel _channel;
        private readonly string _queueName = "microservice_queue";


        public EventBusRabbitMQ(ILogger<EventBusRabbitMQ> logger, IEventBusSubscriptionsManager subsManager,
            IRabbitMQPersistentConnection persistConnection, ILifetimeScope autofac)
        {
            _persistConnection = persistConnection;
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _channel = CreateConsumerChannel();
            _autofac = autofac;

            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistConnection.IsConnected)
            {
                _persistConnection.TryConnect();
            }

            using (var channel = _persistConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);

                if (_subsManager.IsEmpty)
                {
                    // _queueName = string.Empty;
                    _channel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistConnection.IsConnected)
            {
                _persistConnection.TryConnect();
            }

            // 建立Channel
            using (var channel = _persistConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                // 创建直连型(direct)交换器
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: BROKER_NAME, routingKey: eventName, basicProperties: null, body: body);
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);

            if (!containsKey)
            {
                if (!_persistConnection.IsConnected)
                {
                    _persistConnection.TryConnect();
                }

                // 绑定到队列上
                using (var channel = _persistConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);
                }
            }

            // 订阅到内存中
            _subsManager.AddSubscription<T, TH>();
        }

        public void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建消费者模型
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            if (!_persistConnection.IsConnected)
            {
                _persistConnection.TryConnect();
            }

            var channel = _persistConnection.CreateModel();

            // 声明直连交换器
            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            // 声明队列
            channel.QueueDeclare(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            //_queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, e) =>
            {
                var eventName = e.RoutingKey;
                _logger.LogInformation($"Current eventName:{eventName}");
                var message = Encoding.UTF8.GetString(e.Body);

                // 通过Autofac去找绑定的EventHandler
                await ProcessEvent(eventName, message);
            };

            channel.BasicConsume(_queueName, true, consumer);

            channel.CallbackException += (sender, ea) =>
            {
                channel.Dispose();
                channel = CreateConsumerChannel();
            };

            return channel;
        }

        /// <summary>
        /// 调用EventHandler类中的Handle方法进行处理
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ProcessEvent(string eventName, string message)
        {

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }

        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }

            _subsManager.Clear();
        }
    }
}
