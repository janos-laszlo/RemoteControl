using System;
using System.Collections.Generic;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public interface IShutdownCalculator
    {
        DateTime GetNextShutdown(IEnumerable<DateTime> shutdownHistory);
    }
}
