namespace RemoteControlService.ReceiverDevice.Commands
{
    public interface IPowerController
    {
        void ScheduleShutdown(int seconds, bool overrideScheduledShutdown = false);
        void CancelShutdown();
        void Hibernate();
    }
}
