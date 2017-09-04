using Microservice.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusConsole.Events
{
    public class TestEvent2 : IntegrationEvent
    {
        public string Message { get; set; }

        public TestEvent2(string message)
        {
            this.Message = message;
        }
    }
}
