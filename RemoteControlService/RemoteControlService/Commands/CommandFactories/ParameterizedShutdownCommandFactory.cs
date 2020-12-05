using RemoteControlService.Commands;

namespace RemoteControlService.Commands.CommandFactories
{
    public class ParameterizedShutdownCommandFactory : IShutdownCommandFactory
    {
        private readonly IPowerController powerController;

        public ParameterizedShutdownCommandFactory(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public ShutdownCommand CreateShutdownCommand(int seconds, bool overrideScheduledShutdown)
        {
            return new ShutdownCommand(seconds, powerController, overrideScheduledShutdown);
        }
    }
}
