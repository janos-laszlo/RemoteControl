namespace RemoteControlService.Commands
{
    public interface IShutdownCommandFactory
    {
        ShutdownCommand CreateShutdownCommand(int seconds, bool overrideScheduledShutdown);
    }
}