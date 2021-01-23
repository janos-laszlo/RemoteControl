using Domain.Builders;
using Domain.CommandFactories;
using Domain.Commands;
using Domain.Controllers;
using System;

namespace WindowsLibrary.CommandFactories
{
    public class ParameterizedShutdownCommandFactory : IShutdownCommandFactory
    {
        private readonly IPowerController powerController;
        private readonly IShutdownCommandArgumentsBuilder shutdownCommandArgumentsBuilder;

        public ParameterizedShutdownCommandFactory(
            IPowerController powerController,
            IShutdownCommandArgumentsBuilder shutdownCommandArgumentsBuilder)
        {
            this.powerController = powerController;
            this.shutdownCommandArgumentsBuilder = shutdownCommandArgumentsBuilder;
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
                shutdownCommandArgumentsBuilder
                    .WithSeconds(seconds)
                    .ShouldOverrideExistingShutdown(false)
                    .ShouldShowNotification(false));
        }
    }
}
