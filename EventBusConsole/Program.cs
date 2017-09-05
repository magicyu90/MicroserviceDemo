using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusConsole.EventHandlers;
using EventBusConsole.Events;
using Microservice.BuildingBlocks.EventBus;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microservice.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventBusConsole
{
    class Program
    {

        private static string exchangeName = "test_exchange";
        private static string queueName = "microservice_queue";


        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var _logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersisterConnection>>();
                var factory = new ConnectionFactory() { HostName = "localhost" };

                return new DefaultRabbitMQPersisterConnection(factory, _logger);
            });

            services.AddTransient<ICaculateService, CaculateService>();

            //services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            //services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //services.AddTransient<TestEventHandler>();
            //services.AddTransient<TestEvent2Handler>();

            var containerBuilder = new ContainerBuilder();

            // 将原本DI容器中的依赖迁移到Autofac中
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            serviceProvider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information);
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            //var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            //eventBus.Subscribe<TestEvent, TestEventHandler>();
            //eventBus.Subscribe<TestEvent2, TestEvent2Handler>();

            logger.LogInformation("Program starting...");

            var _connection = serviceProvider.GetRequiredService<IRabbitMQPersistentConnection>();
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            #region 简单的工作队列模式
            //using (var channel = _connection.CreateModel())
            //{
            //    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
            //    channel.QueueDeclare(queueName, true, false, false, null);

            //    Console.WriteLine("Listening...");

            //    //在队列上定义一个消费者
            //    var consumer = new EventingBasicConsumer(channel);

            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        Console.WriteLine(" [x] Received {0}", message);
            //        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            //    };

            //    //消费队列，并设置应答模式为程序主动应答
            //    channel.BasicConsume(queueName, false, consumer);

            //    Console.ReadLine();

            //}
            #endregion

            #region 生产者-消费者模式
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                // var queue = channel.QueueDeclare("queueTest", durable: true, exclusive: false, autoDelete: false, arguments: null);
                var queueName = channel.QueueDeclare().QueueName;

                Console.WriteLine("Listening...");

                channel.QueueBind(queueName, exchangeName, "TestEvent");
                channel.QueueBind(queueName, exchangeName, "TestEvent2");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var routingKey = ea.RoutingKey;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    logger.LogInformation("{0}:{1}", routingKey, message);
                    ProcessEvent(container, routingKey, message);
                };

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                Console.ReadLine();
            }

            #endregion
        }

        private static void ProcessEvent(IContainer container, string eventName, string message)
        {
            using (var scope = container.BeginLifetimeScope("HugoScope"))
            {
                var handler = scope.ResolveOptional<ICaculateService>();

                var concreteType = typeof(CaculateService);

                concreteType.GetMethod($"{eventName}Async").Invoke(handler, new object[] { message });
            }
        }


    }
}
