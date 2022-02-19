using Domain.CommandFactories;
using Domain.Commands.Arguments;
using Domain.Common.TaskScheduling;
using Domain.Common.Utilities;
using Domain.Controllers;
using System;

namespace Domain.Commands
{
    public class ShutdownCommand : ICommand
    {
        private readonly IPowerController powerController;
        private readonly ShutdownArgs shutdownArgs;

        public ShutdownCommand(
            IPowerController powerController,
            ShutdownArgs shutdownArgs)
        {

            this.powerController = powerController;
            this.shutdownArgs = shutdownArgs;
        }

        public Maybe<string> Execute()
        {
            powerController.ScheduleShutdown(shutdownArgs);

            if (powerController.NextShutdownDateTime is null)
            {
                return string.Empty;
            }

            return powerController.NextShutdownDateTime.ToString();
        }
    }
}
