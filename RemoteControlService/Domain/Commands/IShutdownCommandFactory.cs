namespace Domain.Commands
{
    public interface IShutdownCommandFactory
    {
        ShutdownCommand CreateShutdownCommand(int seconds, bool overrideScheduledShutdown);
    }
}