using System;
using System.Collections.Generic;

namespace RemoteControlService.NightlyShutdown
{
    public interface IShutdownCalculator
    {
        DateTime GetNextShutdown(IEnumerable<DateTime> shutdownHistory);
    }
}
