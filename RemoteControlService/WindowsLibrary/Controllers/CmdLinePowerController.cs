using Domain.Commands.Arguments;
using Domain.Controllers;
using WindowsLibrary.Builders;
using WindowsLibrary.Utils;

namespace WindowsLibrary.Controllers
{
    public class CmdLinePowerController : IPowerController
    {
        public void CancelShutdown()
        {
            CmdLineUtils.InvokeCommandLineCommand("/C SHUTDOWN /A");
        }

        public void ScheduleShutdown(ShutdownArgs shutdownArgs)
        {
            string args = new ShutdownCommandArgumentsBuilder()
                .Seconds(shutdownArgs.Seconds)
                .OverrideExistingShutdown(shutdownArgs.OverrideExistingShutdown)
                .ShowNotification(shutdownArgs.ShowNotification)
                .Build();

            CmdLineUtils.InvokeCommandLineCommand(args);
        }

        public void Hibernate()
        {
            CmdLineUtils.InvokeCommandLineCommand($"/C SHUTDOWN /H");
        }
    }
}
