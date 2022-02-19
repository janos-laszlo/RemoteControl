using System;

namespace Domain.Commands.Arguments
{
    public class ShutdownArgs
    {
        public DateTime DateTime { get; private set; }
        public bool OverrideExistingShutdown { get; private set; }
        public bool ShowNotification { get; private set; }

        public ShutdownArgs(
            DateTime datetime,
            bool overrideExistingShutdown,
            bool showNotification)
        {
            DateTime = datetime;
            OverrideExistingShutdown = overrideExistingShutdown;
            ShowNotification = showNotification;
        }
    }
}
