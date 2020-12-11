using Domain.Commands;
using Domain.Common.TaskScheduling;
using Domain.NightlyShutdown;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
using System.Threading;
using WindowsLibrary.CommandFactories;
using WindowsLibrary.NightlyShutdown;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class NightlyShutdownSchedulerTest
    {
        private NightlyShutdownScheduler nightlyShutdownScheduler;
        private CmdLinePowerControllerMock powerController;
        private ShutdownHistoryStorage shutdownHistoryStorage;
        private IShutdownCalculator shutdownCalculator;
        private ITaskScheduler taskScheduler;
        private IShutdownCommandFactory shutdownCommandFactory;

        [TestInitialize]
        public void Init()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            powerController = new CmdLinePowerControllerMock();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownCalculator = new NightlyShutdownCalculator();
            taskScheduler = new CommonTaskScheduler();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(powerController);
            nightlyShutdownScheduler = new NightlyShutdownScheduler(shutdownHistoryStorage, shutdownCalculator, taskScheduler, shutdownCommandFactory);
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
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(6));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.IsTrue(powerController.SecondsTillShutdown == 0);
            Thread.Sleep(7000); // Wait for the task within the CommonTaskScheduler to finish
            Assert.AreEqual(600, powerController.SecondsTillShutdown);
        }
    }
}
