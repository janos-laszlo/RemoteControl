using Domain.Controllers;
using WindowsLibrary.Utils;

namespace WindowsLibrary.Controllers
{
    public class CmdLinePowerController : IPowerController
    {
        public void CancelShutdown()
        {
            CmdLineUtils.InvokeCommandLineCommand("/C SHUTDOWN /A");
        }

        public void ScheduleShutdown(string arguments)
        {
            CmdLineUtils.InvokeCommandLineCommand(arguments);
        }

        public void Hibernate()
        {
            CmdLineUtils.InvokeCommandLineCommand($"/C SHUTDOWN /H");
        }
    }
}
