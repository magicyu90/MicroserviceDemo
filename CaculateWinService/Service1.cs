using Autofac;
using IntegrationEventsForStandard.EventHandlers;
using System;
using System.ServiceProcess;

namespace CaculateWinService
{
    public partial class Service1 : ServiceBase
    {

        private readonly string exchangeName = "test_exchange";
        private readonly string queueName = "microservice_queue";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Console.WriteLine("On start...");


            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start.");
            }

            //var containerBuilder = new ContainerBuilder();

            //containerBuilder.RegisterType<CaculateService>().As<ICaculateService>();

            //var container = containerBuilder.Build();


            //var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            //using (IConnection conn = connectionFactory.CreateConnection())
            //using (var channel = conn.CreateModel())
            //{

            //    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            //    // var queue = channel.QueueDeclare("queueTest", durable: true, exclusive: false, autoDelete: false, arguments: null);
            //    var queueName = channel.QueueDeclare().QueueName;

            //    Console.WriteLine("Listening...");

            //    channel.QueueBind(queueName, exchangeName, "TestEvent");
            //    channel.QueueBind(queueName, exchangeName, "TestEvent2");

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var routingKey = ea.RoutingKey;
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        Console.WriteLine("{0}:{1}", routingKey, message);
            //        ProcessEvent(container, routingKey, message);
            //    };

            //    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            //    Console.ReadLine();

            //}


        }

        protected override void OnStop()
        {

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Stop.");
            }
        }

        private void ProcessEvent(Autofac.IContainer container, string eventName, string message)
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
