using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;

namespace Domain.Commands
{
    public class GetNextShutdownCommand : ICommand
    {
        private readonly IPowerController powerController;

        public GetNextShutdownCommand(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public Maybe<string> Execute() =>
            Maybe<string>.Some(powerController.NextShutdownDateTime?.ToString() ?? "--");
    }
}
