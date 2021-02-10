using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;

namespace Domain.Commands
{
    public class GetVolumeCommand : ICommand
    {
        private readonly IVolumeController volumeController;

        public GetVolumeCommand(IVolumeController volumeController)
        {
            this.volumeController = volumeController;
        }

        public Maybe<string> Execute()
        {
            return Maybe<string>.Some(volumeController.VolumeInPercent.ToString());
        }
    }
}
