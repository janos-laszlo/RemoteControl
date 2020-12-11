using Domain.Commands;

namespace Domain.CommandFactories
{
    public interface ITextCommandFactory
    {
        ICommand Create(string unparsedCmd);
    }
}