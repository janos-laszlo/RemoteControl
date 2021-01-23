using Domain.CommandFactories;
using Domain.Common.Utilities;

namespace Domain.Commands
{
    public class GetNextShutdownCommand : ICommand
    {
        public Maybe<string> Execute()
        {
            return Maybe<string>.Some(ShutdownCommand.NextShutdownDateTime.ToString());
        }
    }
}
