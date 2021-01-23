using Domain.CommandFactories;
using Domain.Commands;
using Domain.Commands.Arguments;
using Domain.Controllers;
using System;

namespace WindowsLibrary.CommandFactories
{
    public class ParameterizedShutdownCommandFactory : IShutdownCommandFactory
    {
        private readonly IPowerController powerController;

        public ParameterizedShutdownCommandFactory(
            IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public CancelShutdownCommand CreateCancelShutdownCommand()
        {
            return new CancelShutdownCommand(powerController);
        }

        public ShutdownCommand CreateDailyShutdownCommand(DateTime nextShutdownTime)
        {
            int seconds = (int)(nextShutdownTime - DateTime.Now).TotalSeconds;
            return new ShutdownCommand(
                powerController,
                new ShutdownArgs(
                    seconds,
                    overrideExistingShutdown: false,
                    showNotification: false));
        }
    }
}
