using Domain.Commands;
using Domain.Common.TaskScheduling;
using Domain.NightlyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
using System.Threading;
using WindowsLibrary.CommandFactories;
using WindowsLibrary.NightlyShutdown;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class NightlyShutdownSchedulerTest
    {
        readonly NightlyShutdownScheduler nightlyShutdownScheduler;
        readonly CmdLinePowerControllerMock powerController;
        readonly ShutdownHistoryStorage shutdownHistoryStorage;
        readonly IShutdownCalculator shutdownCalculator;
        readonly ITaskScheduler taskScheduler;
        readonly IShutdownCommandFactory shutdownCommandFactory;

        public NightlyShutdownSchedulerTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            powerController = new CmdLinePowerControllerMock();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownCalculator = new NightlyShutdownCalculator();
            taskScheduler = new CommonTaskScheduler();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(powerController);
            nightlyShutdownScheduler = new NightlyShutdownScheduler(
                shutdownHistoryStorage, 
                shutdownCalculator, 
                taskScheduler, 
                shutdownCommandFactory);
        }

        [Fact]
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
            Assert.Equal(600, powerController.SecondsTillShutdown);
        }

        [Fact]
        public void ScheduleShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDown()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(6));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True(powerController.SecondsTillShutdown == 0);
            Thread.Sleep(7000); // Wait for the task within the CommonTaskScheduler to finish
            Assert.Equal(600, powerController.SecondsTillShutdown);
        }
    }
}
