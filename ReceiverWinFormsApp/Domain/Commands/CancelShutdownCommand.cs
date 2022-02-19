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
            return Maybe<string>.None();
        }
    }
}
