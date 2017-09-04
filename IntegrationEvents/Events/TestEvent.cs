using Microservice.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEvents.Events
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
