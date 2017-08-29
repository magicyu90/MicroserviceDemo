using Microservice.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organization.API.IntegrationEvents.Events
{
    public class DeleteUserEvent :IntegrationEvent
    {
        public int UserId { get; private set; }

        public string UserName { get; private set; }

        public string Mobile { get; private set; }

        public DeleteUserEvent(int userId, string userName, string mobile)
        {
            UserId = userId;
            UserName = UserName;
            Mobile = mobile;
        }
    }
}
