using WindowsLibrary.Controllers;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class CmdLineVolumeControllerTest
    {
        private readonly CmdLineVolumeController volumeController;

        public CmdLineVolumeControllerTest()
        {
            volumeController = new CmdLineVolumeController();
        }

        [Fact]
        public void MyTestMethod()
        {
            volumeController.VolumeInPercent = 16;

            Assert.Equal(16, volumeController.VolumeInPercent);
        }
    }
}
