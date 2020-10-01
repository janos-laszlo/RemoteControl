using RemoteControlService.ReceiverDevice.Commands;

namespace RemoteControlService.ReceiverDevice.Controllers
{
    public class CmdLinePowerController : IPowerController
    {
        public void CancelShutdown()
        {
            CmdLineUtils.InvokeCommandLineCommand("/C SHUTDOWN /A");
        }

        public void ScheduleShutdown(int seconds)
        {
            CmdLineUtils.InvokeCommandLineCommand($"/C SHUTDOWN /A & SHUTDOWN /S /T {seconds}");
        }

        public void Hibernate()
        {
            CmdLineUtils.InvokeCommandLineCommand($"/C SHUTDOWN /H");
        }
    }
}
