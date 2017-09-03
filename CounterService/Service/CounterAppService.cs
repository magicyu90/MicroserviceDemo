using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CounterService.Service
{
    public class CounterAppService : ICounterAppService
    {
        private readonly ILogger _logger;

        public CounterAppService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CounterAppService>();
        }

        public void Count(int loops)
        {
            for (var i = 0; i < loops; i++)
                _logger.LogInformation($"We have got the {i} loop");
        }
    }
}
