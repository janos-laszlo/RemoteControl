namespace Domain.Commands
{
    public class SetVolumeCommand : ICommand
    {
        readonly int percent;
        readonly IVolumeController volumeController;

        public SetVolumeCommand(int percent, IVolumeController volumeController)
        {
            this.percent = percent;
            this.volumeController = volumeController;
        }

        public void Execute()
        {
            volumeController.SetVolume(percent);
        }
    }
}
