using Domain.Commands.Arguments;
using Domain.Controllers;
using Microsoft.Extensions.Logging;
using System;

namespace RemoteControlService.UniTests.Mocks
{
    public class CmdLinePowerControllerMock : IPowerController
    {
        private readonly ILogger<CmdLinePowerControllerMock> logger;

        public CmdLinePowerControllerMock(
            ILogger<CmdLinePowerControllerMock> logger)
        {
            this.logger = logger;
        }

        public int NumOfHibernations { get; private set; }
        public int SecondsTillShutdown { get; private set; }

        public DateTime? NextShutdownDateTime { get; private set; }

        public void CancelShutdown(bool showNotification)
        {
            NextShutdownDateTime = null;
        }

        public void Hibernate()
        {
            ++NumOfHibernations;
        }

        public void ScheduleShutdown(ShutdownArgs arguments)
        {
            logger.LogInformation("Shutdown was called with args: {Arguments}", arguments);
            SecondsTillShutdown = (int)(arguments.DateTime - DateTime.Now).TotalSeconds;
            NextShutdownDateTime = arguments.DateTime;
        }
    }
}