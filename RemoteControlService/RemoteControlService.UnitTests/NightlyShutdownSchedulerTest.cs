using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RemoteControlService.ReceiverDevice.NightlyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class NightlyShutdownSchedulerTest
    {
        private NightlyShutdownScheduler nightlyShutdownScheduler;
        private Mock<ISystemInformation> sysInfoMock;
        private ShutdownHistoryStorage shutdownHistoryStorage;
        private CmdLinePowerControllerMock powerController;
        private IShutdownCalculator shutdownCalculator;

        [TestInitialize]
        public void Init()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            sysInfoMock = new Mock<ISystemInformation>();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            powerController = new CmdLinePowerControllerMock();
            shutdownCalculator = new NightlyShutdownCalculator();
            nightlyShutdownScheduler = new NightlyShutdownScheduler(shutdownHistoryStorage, powerController, sysInfoMock.Object, shutdownCalculator);
        }

        [ClassCleanup]
        public static void CleanClass()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
        }

        [TestMethod]
        public async Task ScheduleShutdown_WhenLessThan10MinutesTillShutdown_ThenShutdownScheduledImmediatelyWith10MinuteDelay()
        {
            var d = DateTime.Now;
            var d1 = d.AddDays(-1);
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d1.AddSeconds(3));
            shutdownHistoryStorage.Add(d1.AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddSeconds(5));
            shutdownHistoryStorage.Add(d1.AddSeconds(6));

            await nightlyShutdownScheduler.ScheduleShutdown();

            Assert.IsTrue(powerController.SecondsTillShutdown <= 600);
            Assert.IsTrue(powerController.SecondsTillShutdown >= 540);
        }

        [TestMethod]
        public async Task ScheduleShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDOwn()
        {
            var d = DateTime.Now;
            var d1 = d.AddDays(-2);
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddMinutes(10).AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await nightlyShutdownScheduler.ScheduleShutdown();
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 6100);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds > 5000);
            Assert.IsTrue(powerController.SecondsTillShutdown <= 600);
            Assert.IsTrue(powerController.SecondsTillShutdown > 540);
        }
    }
}
