using Autofac;
using Autofac.Extensions.DependencyInjection;
using CounterService.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CounterService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var serviceCollecion = new ServiceCollection();

            // DI容器中添加Logging组件(LoggerFactory)
            serviceCollecion.AddLogging();
            
            var containerBuilder = new ContainerBuilder();
            // 将DI容器中的组件传递到Autofac容器内
            containerBuilder.Populate(serviceCollecion);

            containerBuilder.RegisterType<CounterAppService>().As<ICounterAppService>();

            // 创建Autofac容器提供者服务
            var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

            serviceProvider.GetService<ILoggerFactory>().AddConsole(LogLevel.Debug);
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Starting");

            var counter = serviceProvider.GetService<ICounterAppService>();

            counter.Count(10);

            logger.LogInformation("Ending!");

            Console.Read();

        }
    }
}
