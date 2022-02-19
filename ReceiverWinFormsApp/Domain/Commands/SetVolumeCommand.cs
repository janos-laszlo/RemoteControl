using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;

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

        public Maybe<string> Execute()
        {
            volumeController.VolumeInPercent = percent;
            return Maybe<string>.None();
        }
    }
}
