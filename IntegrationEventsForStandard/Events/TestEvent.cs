using IntegrationEventsForStandard.Events;
using System;

namespace IntegrationEventsForStandard
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
