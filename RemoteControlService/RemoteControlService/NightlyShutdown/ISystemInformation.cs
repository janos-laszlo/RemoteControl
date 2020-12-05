using System;

namespace RemoteControlService.NightlyShutdown
{
    public interface ISystemInformation
    {
        DateTime GetLastSystemShutdown();
    }
}