namespace Domain.Commands.Arguments
{
    public class ShutdownArgs
    {
        public int Seconds { get; private set; }
        public bool OverrideExistingShutdown { get; private set; }
        public bool ShowNotification { get; private set; }

        public ShutdownArgs(
            int seconds,
            bool overrideExistingShutdown,
            bool showNotification)
        {
            Seconds = seconds;
            OverrideExistingShutdown = overrideExistingShutdown;
            ShowNotification = showNotification;
        }
    }
}
