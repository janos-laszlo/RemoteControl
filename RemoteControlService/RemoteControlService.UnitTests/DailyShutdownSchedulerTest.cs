using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RemoteControlService.ReceiverDevice.DailyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class DailyShutdownSchedulerTest
    {
        private DailyShutdownScheduler dailyShutdownScheduler;
        private Mock<ISystemInformation> sysInfoMock;
        private ShutdownHistoryStorage shutdownHistoryStorage;
        private CmdLinePowerControllerMock powerController;

        [TestInitialize]
        public void Init()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            sysInfoMock = new Mock<ISystemInformation>();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            powerController = new CmdLinePowerControllerMock();
            dailyShutdownScheduler = new DailyShutdownScheduler(shutdownHistoryStorage, powerController, sysInfoMock.Object);
        }

        [ClassCleanup]
        public static void CleanClass()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
        }

        [TestMethod]
        public async Task ScheduleDailyShutdown_WhenLessThan10MinutesTillShutdown_ThenShutdownScheduledImmediatelyWith10MinuteDelay()
        {
            var d = DateTime.Now;
            var d1 = d.AddDays(-1);
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d1.AddSeconds(3));
            shutdownHistoryStorage.Add(d1.AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddSeconds(5));
            shutdownHistoryStorage.Add(d1.AddSeconds(6));

            await dailyShutdownScheduler.ScheduleDailyShutdown();

            Assert.IsTrue(powerController.SecondsTillShutdown <= 600);
            Assert.IsTrue(powerController.SecondsTillShutdown >= 540);
        }

        [TestMethod]
        public async Task ScheduleDailyShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDOwn()
        {
            var d = DateTime.Now;
            var d1 = d.AddDays(-2);
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddMinutes(11));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await dailyShutdownScheduler.ScheduleDailyShutdown();
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds <= 6100);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds > 5000);
            Assert.IsTrue(powerController.SecondsTillShutdown <= 600);
            Assert.IsTrue(powerController.SecondsTillShutdown > 540);
        }
    }
}
