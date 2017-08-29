using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Organization.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organization.API.IntegrationEvents.Handlers
{
    public class DeleteUserEventHandler : IIntegrationEventHandler<DeleteUserEvent>
    {

        private readonly ILogger _logger;

        public DeleteUserEventHandler(ILogger<DeleteUserEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(DeleteUserEvent @event)
        {
            _logger.LogInformation("Get Message:{0}", @event);
            await Task.FromResult<object>(null);
        }
    }
}
