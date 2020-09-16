namespace RemoteControlService.ReceiverDevice.Commands
{
    public interface IPowerController
    {
        void ScheduleShutdown(int seconds);
        void CancelShutdown();
        void Hibernate();
    }
}
