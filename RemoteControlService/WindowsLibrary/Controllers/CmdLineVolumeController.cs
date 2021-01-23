using Domain.Controllers;
using WindowsLibrary.Utils;

namespace WindowsLibrary.Controllers
{
    public class CmdLineVolumeController : IVolumeController
    {
        public void SetVolume(int percent)
        {
            string volume = (65535 * percent / 100).ToString();
            CmdLineUtils.InvokeCommandLineCommand($"/C NIRCMD SETSYSVOLUME {volume}");
        }
    }
}
