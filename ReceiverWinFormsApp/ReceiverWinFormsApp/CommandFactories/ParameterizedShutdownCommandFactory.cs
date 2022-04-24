using Domain.CommandFactories;
using Domain.Commands;
using Domain.Commands.Arguments;
using Domain.Controllers;
using System;

namespace ReceiverWinFormsApp.CommandFactories
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
            return new CancelShutdownCommand(powerController, false);
        }

        public ShutdownCommand CreateDailyShutdownCommand(DateTime nextShutdownTime)
        {
            return new ShutdownCommand(
                powerController,
                new ShutdownArgs(
                    nextShutdownTime,
                    overrideExistingShutdown: false,
                    showNotification: true));
        }
    }
}
