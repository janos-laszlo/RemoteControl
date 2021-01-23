using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;
using System;

namespace Domain.Commands
{
    public class CancelShutdownCommand : ICommand
    {
        private readonly IPowerController powerController;

        public CancelShutdownCommand(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public Maybe<string> Execute()
        {
            powerController.CancelShutdown();
            ShutdownCommand.NextShutdownDateTime = Maybe<DateTime>.None();
            return Maybe<string>.None();
        }
    }
}
