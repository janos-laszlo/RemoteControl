using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;
using System;

namespace Domain.Commands
{
    public class CancelShutdownCommand : ICommand
    {
        private readonly IPowerController powerController;
        private readonly bool showNotification;

        public CancelShutdownCommand(IPowerController powerController, bool showNotification)
        {
            this.powerController = powerController;
            this.showNotification = showNotification;
        }

        public Maybe<string> Execute()
        {
            powerController.CancelShutdown(showNotification);
            return Maybe<string>.None();
        }
    }
}
