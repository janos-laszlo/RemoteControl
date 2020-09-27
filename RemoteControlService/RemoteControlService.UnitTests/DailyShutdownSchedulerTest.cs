using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RemoteControlService.ReceiverDevice.DailyShutdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class DailyShutdownSchedulerTest
    {
        private Mock<ISystemInformation> sysInfoMock;
        private DailyShutdownScheduler dailyShutdownScheduler;
        private ShutdownHistoryStorage shutdownHistoryStorage;

        [TestInitialize]
        public void Init()
        {
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            sysInfoMock = new Mock<ISystemInformation>();
            dailyShutdownScheduler = new DailyShutdownScheduler(shutdownHistoryStorage, new CmdLinePowerControllerMock(), sysInfoMock.Object);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
        }

        [TestMethod]
        public void UpdateShutdownHistory_WhenLastSystemShutdownOccuredBetween10AMAnd5PM_ThenAddedShutdownHistoryStorage()
        {
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 22, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 23, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 0, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 1, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 2, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();

            var times = shutdownHistoryStorage.GetAll();
            Assert.AreEqual(5, times.Count());
        }

        [TestMethod]
        public void UpdateShutdownHistory_WhenMoreThan5ShutdownsOccuredBetween10AMAnd5PM_ThenMax5AddedShutdownHistoryStorage()
        {
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 0, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 1, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 2, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 3, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 13, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 4, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 22, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(new DateTime(2020, 9, 26, 23, 10, 0));
            dailyShutdownScheduler.UpdateShutdownHistory();

            var times = shutdownHistoryStorage.GetAll();
            Assert.AreEqual(5, times.Count());
            Assert.IsTrue(times.Contains(new DateTime(2020, 9, 26, 23, 10, 0)));
            Assert.IsTrue(times.Contains(new DateTime(2020, 9, 26, 2, 10, 0)));
            Assert.IsTrue(!times.Contains(new DateTime(2020, 9, 26, 1, 10, 0)));
            Assert.IsTrue(!times.Contains(new DateTime(2020, 9, 26, 0, 10, 0)));
            Assert.IsTrue(!times.Contains(new DateTime(2020, 9, 26, 13, 10, 0)));
        }

        [TestMethod]
        public void ScheduleDailyShutdown_WhenLessThan10MinutesTillShutdown_ThenShutdownScheduledImmediatelyWith10MinuteDelay()
        {
            var shutdownHistoryMock = new Mock<IShutdownHistoryStorage>();
            var d = DateTime.Now;
            shutdownHistoryMock.Setup(s => s.GetAll()).Returns(new List<DateTime>
            {
                d.AddMinutes(1),
                d.AddMinutes(2),
                d.AddMinutes(3),
                d.AddMinutes(4),
                d.AddMinutes(5),
            });

            var dailyShutdownScheduler1 = new DailyShutdownScheduler(shutdownHistoryMock.Object, new CmdLinePowerControllerMock(), sysInfoMock.Object);
            dailyShutdownScheduler1.ScheduleDailyShutdown();

            // Check output.
        }

        [TestMethod]
        public void ScheduleDailyShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDOwn()
        {
            var shutdownHistoryMock = new Mock<IShutdownHistoryStorage>();
            var d = DateTime.Now;
            shutdownHistoryMock.Setup(s => s.GetAll()).Returns(new List<DateTime>
            {
                d.AddMinutes(10),
                d.AddMinutes(10),
                d.AddMinutes(10),
                d.AddMinutes(12),
                d.AddMinutes(13),
            });

            var dailyShutdownScheduler1 = new DailyShutdownScheduler(shutdownHistoryMock.Object, new CmdLinePowerControllerMock(), sysInfoMock.Object);
            dailyShutdownScheduler1.ScheduleDailyShutdown();

            // Check output.
        }
    }
}
