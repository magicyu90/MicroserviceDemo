using IntegrationEvents.Events;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microservice.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
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


        private readonly string exchangeName = "microservice_exchange";
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
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var eventMessage = new TestEvent("Test event...");

                var eventMessage2 = new TestEvent2("Test event2...");

                var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage));
                var msgBody2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage2));

                channel.BasicPublish("", queueName, null, msgBody);
                channel.BasicPublish("", queueName, null, msgBody2);
            }

            return Json("OK");

        }
    }
}
