using Domain.CommandFactories;
using Domain.Commands.Arguments;
using Domain.Common.Utilities;
using Domain.Controllers;
using System;

namespace Domain.Commands
{
    public class ShutdownCommand : ICommand
    {
        public static Maybe<DateTime> NextShutdownDateTime = Maybe<DateTime>.None();
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
            NextShutdownDateTime = Maybe<DateTime>.Some(
                DateTime.Now.AddSeconds(shutdownArgs.Seconds));
            return Maybe<string>.None();
        }
    }
}
