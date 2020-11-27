using System;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public interface ISystemInformation
    {
        DateTime GetLastSystemShutdown();
    }
}