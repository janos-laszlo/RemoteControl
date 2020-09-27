using System;

namespace RemoteControlService.ReceiverDevice.DailyShutdown
{
    public interface ISystemInformation
    {
        DateTime GetLastSystemShutdown();
    }
}