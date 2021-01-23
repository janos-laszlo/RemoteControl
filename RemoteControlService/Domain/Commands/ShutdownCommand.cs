using Domain.Builders;
using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;
using System;

namespace Domain.Commands
{
    public class ShutdownCommand : ICommand
    {
        public static readonly DateTime NextShutdownDateTime = DateTime.MinValue;
        private readonly IPowerController powerController;
        private readonly IShutdownCommandArgumentsBuilder shutdownCommandArgumentsBuilder;

        public ShutdownCommand(
            IPowerController powerController,
            IShutdownCommandArgumentsBuilder shutdownCommandArgumentsBuilder)
        {

            this.powerController = powerController;
            this.shutdownCommandArgumentsBuilder = shutdownCommandArgumentsBuilder;
        }

        public Maybe<string> Execute()
        {
            powerController.ScheduleShutdown(shutdownCommandArgumentsBuilder.Build());
            return Maybe<string>.None();
        }
    }
}
