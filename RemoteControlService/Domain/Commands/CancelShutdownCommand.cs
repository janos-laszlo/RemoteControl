namespace Domain.Commands
{
    public class CancelShutdownCommand : ICommand
    {
        private readonly IPowerController powerController;

        public CancelShutdownCommand(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public void Execute()
        {
            powerController.CancelShutdown();
        }
    }
}
