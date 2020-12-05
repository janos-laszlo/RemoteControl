using RemoteControlService.Commands;
using System.Diagnostics;

namespace RemoteControlService.UniTests.Mocks
{
    internal class CmdLinePowerControllerMock : IPowerController
    {
        public int NumOfCancelledShutdowns { get; private set; }
        public int NumOfHibernations { get; private set; }
        public int SecondsTillShutdown { get; private set; }

        public void CancelShutdown()
        {
            ++NumOfCancelledShutdowns;
        }

        public void Hibernate()
        {
            ++NumOfHibernations;
        }

        public void ScheduleShutdown(int seconds, bool overrideScheduledShutdown = false)
        {
            Trace.TraceInformation($"Shutdown was scheduled to happen in {seconds} seconds");
            SecondsTillShutdown = seconds;
        }
    }
}