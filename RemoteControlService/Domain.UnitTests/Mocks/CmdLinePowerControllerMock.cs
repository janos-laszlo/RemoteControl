using Domain.Commands.Arguments;
using Domain.Controllers;
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

        public void ScheduleShutdown(ShutdownArgs arguments)
        {
            Trace.TraceInformation($"Shutdown was called with args: {arguments}");
            SecondsTillShutdown = arguments.Seconds;
        }
    }
}