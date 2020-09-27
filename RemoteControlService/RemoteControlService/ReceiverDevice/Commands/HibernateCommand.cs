namespace RemoteControlService.ReceiverDevice.Commands
{
    public class HibernateCommand : ICommand
    {
        private readonly IPowerController powerController;

        public HibernateCommand(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public void Execute()
        {
            powerController.Hibernate();
        }
    }
}
