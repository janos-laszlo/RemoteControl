namespace Domain.Controllers
{
    public interface IPowerController
    {
        void ScheduleShutdown(string arguments);
        void CancelShutdown();
        void Hibernate();
    }
}
