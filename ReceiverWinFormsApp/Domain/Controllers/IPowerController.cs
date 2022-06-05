using Domain.Commands.Arguments;
using System;

namespace Domain.Controllers
{
    public interface IPowerController
    {
        DateTime? NextShutdownDateTime { get; }
        void ScheduleShutdown(ShutdownArgs arguments);
        void CancelShutdown(bool showNotification);
        void Hibernate();
        event Action<DateTime?> NextShutdownChanged;
    }
}
