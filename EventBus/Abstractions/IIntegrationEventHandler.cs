using Microservice.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntergrationEvent> : IIntegrationEventHandler
        where TIntergrationEvent : IntegrationEvent
    {
        Task Handle(TIntergrationEvent @event);
    }


    public interface IIntegrationEventHandler
    {
    }
}
