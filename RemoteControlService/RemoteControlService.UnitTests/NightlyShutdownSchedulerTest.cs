using Domain.Builders;
using Domain.CommandFactories;
using Domain.NightlyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
using WindowsLibrary.Builders;
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
        readonly IShutdownCommandFactory shutdownCommandFactory;
        readonly IShutdownCommandArgumentsBuilder shutdownArgumentsBuilder;

        public NightlyShutdownSchedulerTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            powerController = new CmdLinePowerControllerMock();
            shutdownArgumentsBuilder = new WindowsShutdownCommandArgumentsBuilder();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownCalculator = new NightlyShutdownCalculator();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(
                powerController, shutdownArgumentsBuilder);
            nightlyShutdownScheduler = new NightlyShutdownScheduler(
                shutdownHistoryStorage,
                shutdownCalculator,
                shutdownCommandFactory);
        }

        [Fact]
        public void ScheduleShutdown_WhenLessThan10MinutesTillShutdown_ThenShutdownScheduledImmediatelyWith10MinuteDelay()
        {
            var d1 = DateTime.Now.AddDays(-1);
            shutdownHistoryStorage.Add(DateTime.Now.AddSeconds(2));
            shutdownHistoryStorage.Add(d1.AddSeconds(3));
            shutdownHistoryStorage.Add(d1.AddSeconds(4));
            shutdownHistoryStorage.Add(DateTime.Now.AddSeconds(5));
            shutdownHistoryStorage.Add(d1.AddSeconds(6));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True("/C SHUTDOWN /S /T 600 /c \" \"" == powerController.Arguments ||
                        "/C SHUTDOWN /S /T 599 /c \" \"" == powerController.Arguments);
        }

        [Fact]
        public void ScheduleShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDown()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(6));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True("/C SHUTDOWN /S /T 606 /c \" \"" == powerController.Arguments ||
                        "/C SHUTDOWN /S /T 605 /c \" \"" == powerController.Arguments);
        }
    }
}
