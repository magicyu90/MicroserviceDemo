using System;
using System.Collections.Generic;
using System.Text;

namespace CounterService.Service
{
    public interface ICounterAppService
    {
        void Count(int loops);
    }
}
