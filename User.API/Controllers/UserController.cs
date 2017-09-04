using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using User.API.Repositories;
using User.API.IntegrationEvents.Events;
using IntegrationEvents.Events;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IEventBus _eventBus;
        private readonly IUserRepository _userRepository;

        private readonly ILogger _logger;


        public UserController(IEventBus eventBus, IUserRepository userRepository, ILogger<UserController> logger)
        {
            _eventBus = eventBus;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            _logger.LogInformation($"Delete id:{id}");

            var testEventMessage = new IntegrationEvents.Events.TestEvent("This is the test message for test event by Hugo");
            _eventBus.Publish(testEventMessage);

            var testEvent2Message = new IntegrationEvents.Events.TestEvent2("This is the test message for test event2 by Hugo");
            _eventBus.Publish(testEvent2Message);


            return Json("OK");

        }
    }
}
