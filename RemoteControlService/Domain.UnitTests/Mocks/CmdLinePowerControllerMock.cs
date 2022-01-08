using Domain.Commands.Arguments;
using Domain.Controllers;
using Microsoft.Extensions.Logging;

namespace RemoteControlService.UniTests.Mocks
{
    internal class CmdLinePowerControllerMock : IPowerController
    {
        private readonly ILogger<CmdLinePowerControllerMock> logger;

        public CmdLinePowerControllerMock(
            ILogger<CmdLinePowerControllerMock> logger)
        {
            this.logger = logger;
        }

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
            logger.LogInformation($"Shutdown was called with args: {arguments}");
            SecondsTillShutdown = arguments.Seconds;
        }
    }
}