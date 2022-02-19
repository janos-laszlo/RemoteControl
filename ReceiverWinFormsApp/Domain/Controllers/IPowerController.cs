using Domain.Commands.Arguments;
using System;

namespace Domain.Controllers
{
    public interface IPowerController
    {
        DateTime? NextShutdownDateTime { get; }
        void ScheduleShutdown(ShutdownArgs arguments);
        void CancelShutdown();
        void Hibernate();
    }
}
