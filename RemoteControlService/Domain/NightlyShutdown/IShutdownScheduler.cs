namespace Domain.NightlyShutdown
{
    public interface IShutdownScheduler
    {
        void CancelShutdown();
        void ScheduleShutdown();
    }
}