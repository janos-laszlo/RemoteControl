using Domain.Common.Utilities;

namespace Domain.CommandFactories
{
    public interface ICommand
    {
        Maybe<string> Execute();
    }
}
