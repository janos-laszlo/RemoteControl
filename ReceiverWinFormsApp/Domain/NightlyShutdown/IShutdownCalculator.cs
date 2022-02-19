using System;
using System.Collections.Generic;

namespace Domain.NightlyShutdown
{
    public interface IShutdownCalculator
    {
        DateTime GetNextShutdown(IEnumerable<DateTime> shutdownHistory);
    }
}
