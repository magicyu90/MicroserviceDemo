using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBusConsole.EventHandlers
{
    public class CaculateService : ICaculateService
    {
        private readonly ILogger<CaculateService> _logger;

        public CaculateService(ILogger<CaculateService> logger)
        {
            _logger = logger;
        }

        public async Task CaculateTrainingProgessAsync(string message)
        {
            _logger.LogInformation("Cacaulte training progress...");
            await Task.FromResult<object>(null);
        }

        public async Task TestEventAsync(string message)
        {
            _logger.LogInformation("Handle test event...");
            await Task.FromResult<object>(null);
        }

        public async Task TestEvent2Async(string message)
        {
            _logger.LogInformation("Handle test event2...");
            await Task.FromResult<object>(null);
        }
    }
}
