namespace Domain.CommandFactories
{
    public interface ITextCommandFactory
    {
        ICommand Create(string command);
    }
}