using RemoteControlService.ReceiverDevice.Commands;

namespace RemoteControlService.ReceiverDevice.Controllers
{
    class CmdLineVolumeController : IVolumeController
    {
        public void SetVolume(int percent)
        {
            string volume = ((65535 * percent) / 100).ToString();
            CmdLineUtils.InvokeCommandLineCommand($"/C NIRCMD SETSYSVOLUME {volume}");
        }
    }
}
