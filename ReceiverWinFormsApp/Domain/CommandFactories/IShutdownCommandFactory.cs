using Domain.Commands;
using System;

namespace Domain.CommandFactories
{
    public interface IShutdownCommandFactory
    {
        CancelShutdownCommand CreateCancelShutdownCommand();
        ShutdownCommand CreateDailyShutdownCommand(DateTime nextShutdownTime);
    }
}