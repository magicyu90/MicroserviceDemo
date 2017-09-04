using Microservice.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.IntegrationEvents.Events
{
    public class TestEvent : IntegrationEvent
    {
        public string Message { get; set; }
        public TestEvent(string message)
        {
            this.Message = message;
        }
    }
}
