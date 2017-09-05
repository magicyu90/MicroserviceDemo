using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBusConsole.EventHandlers
{
    public interface ICaculateService
    {
        Task CaculateTrainingProgessAsync(string message);

        Task TestEventAsync(string message);

        Task TestEvent2Async(string message);
    }
}
