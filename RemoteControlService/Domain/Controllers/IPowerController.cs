using Domain.Commands.Arguments;

namespace Domain.Controllers
{
    public interface IPowerController
    {
        void ScheduleShutdown(ShutdownArgs arguments);
        void CancelShutdown();
        void Hibernate();
    }
}
