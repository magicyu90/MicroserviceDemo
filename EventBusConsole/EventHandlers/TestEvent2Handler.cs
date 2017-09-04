using EventBusConsole.Events;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBusConsole.EventHandlers
{
    public class TestEvent2Handler : IIntegrationEventHandler<TestEvent2>
    {
        private readonly ILogger _logger;

        public TestEvent2Handler(ILogger<TestEvent2Handler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TestEvent2 @event)
        {
            _logger.LogInformation($"Get Message in testEvent2Handler:{@event.Message}");
            await Task.FromResult<object>(null);
        }

    
    }
}
