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
using System;

namespace EventBusConsole
{
    class Program
    {
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

            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<TestEventHandler>();
            services.AddTransient<TestEvent2Handler>();

            var containerBuilder = new ContainerBuilder();

            // 将原本DI容器中的依赖迁移到Autofac中
            containerBuilder.Populate(services);

            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            serviceProvider.GetService<ILoggerFactory>().AddConsole(LogLevel.Information);
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<TestEvent, TestEventHandler>();
            eventBus.Subscribe<TestEvent2, TestEvent2Handler>();

            logger.LogInformation("Program starting...");

        }
    }
}
