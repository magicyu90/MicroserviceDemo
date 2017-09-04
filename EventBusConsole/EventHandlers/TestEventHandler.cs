using EventBusConsole.Events;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBusConsole.EventHandlers
{
    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        private readonly ILogger _logger;

        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TestEvent @event)
        {
            _logger.LogInformation($"Get Message:{@event.Message}");
            await Task.FromResult<object>(null);
        }
    }
}
