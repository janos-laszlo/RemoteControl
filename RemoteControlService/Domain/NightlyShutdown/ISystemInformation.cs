using System;

namespace Domain.NightlyShutdown
{
    public interface ISystemInformation
    {
        DateTime GetLastSystemShutdown();
    }
}