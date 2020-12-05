using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RemoteControlService.Commands;
using RemoteControlService.Commands.CommandFactories;
using RemoteControlService.Common.TaskScheduling;
using RemoteControlService.NightlyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
using System.Threading;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class NightlyShutdownSchedulerTest
    {
        private NightlyShutdownScheduler nightlyShutdownScheduler;
        private Mock<ISystemInformation> sysInfoMock;
        private CmdLinePowerControllerMock powerController;
        private ShutdownHistoryStorage shutdownHistoryStorage;
        private IShutdownCalculator shutdownCalculator;
        private ITaskScheduler taskScheduler;
        private IShutdownCommandFactory shutdownCommandFactory;

        [TestInitialize]
        public void Init()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            sysInfoMock = new Mock<ISystemInformation>();
            powerController = new CmdLinePowerControllerMock();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownCalculator = new NightlyShutdownCalculator();
            taskScheduler = new CommonTaskScheduler();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(powerController);
            nightlyShutdownScheduler = new NightlyShutdownScheduler(shutdownHistoryStorage, sysInfoMock.Object, shutdownCalculator, taskScheduler, shutdownCommandFactory);
        }

        [ClassCleanup]
        public static void CleanClass()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
        }

        [TestMethod]
        public void ScheduleShutdown_WhenLessThan10MinutesTillShutdown_ThenShutdownScheduledImmediatelyWith10MinuteDelay()
        {
            var d = DateTime.Now;
            var d1 = d.AddDays(-1);
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d.AddSeconds(2));
            shutdownHistoryStorage.Add(d1.AddSeconds(3));
            shutdownHistoryStorage.Add(d1.AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddSeconds(5));
            shutdownHistoryStorage.Add(d1.AddSeconds(6));

            nightlyShutdownScheduler.ScheduleShutdown();

            Thread.Sleep(100); // Wait for the task within the CommonTaskScheduler to finish
            Assert.AreEqual(600, powerController.SecondsTillShutdown);
        }

        [TestMethod]
        public void ScheduleShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDown()
        {
            var d = DateTime.Now;
            sysInfoMock.Setup(s => s.GetLastSystemShutdown()).Returns(d.AddMinutes(10).AddSeconds(4));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(6));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.IsTrue(powerController.SecondsTillShutdown == 0);
            Thread.Sleep(7000); // Wait for the task within the CommonTaskScheduler to finish
            Assert.AreEqual(600, powerController.SecondsTillShutdown);
        }
    }
}
