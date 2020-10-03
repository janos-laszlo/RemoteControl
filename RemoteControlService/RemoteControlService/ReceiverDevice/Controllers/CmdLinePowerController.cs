using RemoteControlService.ReceiverDevice.Commands;

namespace RemoteControlService.ReceiverDevice.Controllers
{
    public class CmdLinePowerController : IPowerController
    {
        public void CancelShutdown()
        {
            CmdLineUtils.InvokeCommandLineCommand("/C SHUTDOWN /A");
        }

        public void ScheduleShutdown(int seconds, bool overrideScheduledShutdown)
        {
            string arguments = overrideScheduledShutdown ? $"/C SHUTDOWN /A & SHUTDOWN /S /T {seconds}" : $"/C SHUTDOWN /S /T {seconds}";
            CmdLineUtils.InvokeCommandLineCommand(arguments);
        }

        public void Hibernate()
        {
            CmdLineUtils.InvokeCommandLineCommand($"/C SHUTDOWN /H");
        }
    }
}
