using WindowsLibrary.Controllers;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class CmdLineVolumeControllerTest
    {
        private readonly NAudioVolumeController volumeController;

        public CmdLineVolumeControllerTest()
        {
            volumeController = new NAudioVolumeController();
        }

        [Fact]
        public void MyTestMethod()
        {
            volumeController.VolumeInPercent = 16;

            Assert.Equal(16, volumeController.VolumeInPercent);
        }
    }
}
