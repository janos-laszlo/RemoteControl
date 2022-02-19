using Domain;
using Domain.CommandFactories;
using Domain.NightlyShutdown;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
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

        public NightlyShutdownSchedulerTest()
        {
            var locations = new Locations(".", "shutdown history.txt", "shutdownHistory.json");

            File.Delete(locations.ShutdownHistoryFilePath);
            powerController = new CmdLinePowerControllerMock(Substitute.For<ILogger<CmdLinePowerControllerMock>>());
            shutdownHistoryStorage = new ShutdownHistoryStorage(locations);
            shutdownCalculator = new NightlyShutdownCalculator();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(
                powerController);
            nightlyShutdownScheduler = new NightlyShutdownScheduler(
                shutdownHistoryStorage,
                shutdownCalculator,
                shutdownCommandFactory,
                Substitute.For<ILogger<NightlyShutdownScheduler>>());
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

            Assert.True(599 <= powerController.SecondsTillShutdown &&
                        powerController.SecondsTillShutdown <= 600);
        }

        [Fact]
        public void ScheduleShutdown_WhenMoreThan10MinutesTillShutdown_ThenShutdownScheduled10MinutesBeforeShuttingDown()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(6));
            shutdownHistoryStorage.Add(d.AddMinutes(10).AddSeconds(8));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True(605 <= powerController.SecondsTillShutdown &&
                        powerController.SecondsTillShutdown <= 606);
        }
    }
}
