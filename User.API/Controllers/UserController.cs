using IntegrationEvents.Events;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microservice.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using User.API.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        // private readonly IEventBus _eventBus;
        private readonly IUserRepository _userRepository;

        private readonly ILogger _logger;
        private readonly IRabbitMQPersistentConnection _connection;


        private readonly string exchangeName = "test_exchange";
        private readonly string queueName = "microservice_queue";


        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IRabbitMQPersistentConnection rabbitMQConnection)
        {
            //_eventBus = eventBus;
            _userRepository = userRepository;
            _logger = logger;
            _connection = rabbitMQConnection;

        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            _logger.LogInformation($"Delete id:{id}");

            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            #region 简单的工作队列模式
            //using (var channel = _connection.CreateModel())
            //{
            //    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            //    var eventMessage = new TestEvent("Test event...");

            //    var eventMessage2 = new TestEvent2("Test event2...");

            //    var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage));
            //    var msgBody2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage2));

            //    channel.BasicPublish("", queueName, null, msgBody);
            //    channel.BasicPublish("", queueName, null, msgBody2);
            //}
            #endregion


            #region 生产者-消费者模式
            //using (var channel = _connection.CreateModel())
            //{
            //    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            //    var eventMesage = new TestEvent("Test Event");
            //    var eventMessage2 = new TestEvent2("Test event2...");

            //    var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMesage));
            //    var msgBody2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage2));

            //    channel.BasicPublish(exchangeName, "TestEvent", null, msgBody);
            //    channel.BasicPublish(exchangeName, "TestEvent2", null, msgBody2);
            //}


            #endregion


            #region 延迟发送

            using (var channel = _connection.CreateModel())
            {
                Dictionary<string, object> arguments = new Dictionary<string, object>();
                arguments.Add("x-delayed-type", "direct");

                channel.ExchangeDeclare(exchangeName, "x-delayed-message", true, false, arguments);

                var eventMesage = new TestEvent("Delayed message...");
                var eventMessage2 = new TestEvent2("Direct message without delay");

                var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMesage));
                var msgBody2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage2));

                // 建立 BasicProperties
                var props = channel.CreateBasicProperties();
                // 建立 diciionary 來放 header 內容
                Dictionary<string, object> headers = new Dictionary<string, object>();
                // 指定 x-delay 及 延遲毫秒數
                headers.Add("x-delay", 5000);
                // 將定義完成的 dictionary 指定給 BasicProperties 的 header
                props.Headers = headers;
             

                channel.BasicPublish(exchangeName, "TestEvent", props, msgBody);
                channel.BasicPublish(exchangeName, "TestEvent2", null, msgBody2);

            }

            #endregion



            return Json("OK");

        }
    }
}
