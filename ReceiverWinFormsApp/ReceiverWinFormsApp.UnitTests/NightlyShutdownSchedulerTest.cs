using Domain;
using Domain.CommandFactories;
using Domain.Commands;
using Domain.Commands.Arguments;
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
        readonly IShutdownHistoryStorage shutdownHistoryStorage;
        readonly IShutdownCalculator shutdownCalculator;
        readonly IShutdownCommandFactory shutdownCommandFactory;

        public NightlyShutdownSchedulerTest()
        {
            powerController = new CmdLinePowerControllerMock(Substitute.For<ILogger<CmdLinePowerControllerMock>>());
            shutdownHistoryStorage = Substitute.For<IShutdownHistoryStorage>();
            shutdownCalculator = new NightlyShutdownCalculator();
            shutdownCommandFactory = Substitute.For<IShutdownCommandFactory>();
            nightlyShutdownScheduler = new NightlyShutdownScheduler(
                shutdownHistoryStorage,
                shutdownCalculator,
                shutdownCommandFactory,
                Substitute.For<ILogger<NightlyShutdownScheduler>>());
        }

        [Fact]
        public void ShutdownShouldOccur10MinutesFromNowWhenLessThan10MinutesTillScheduledTime()
        {
            var d1 = DateTime.Now.AddDays(-1);
            shutdownHistoryStorage.GetAll()
                .Returns(new[]
                {
                    DateTime.Now.AddSeconds(2),
                    d1.AddSeconds(3),
                    d1.AddSeconds(4),
                    DateTime.Now.AddSeconds(5),
                    d1.AddSeconds(6)
                });

            shutdownCommandFactory.CreateDailyShutdownCommand(Arg.Any<DateTime>())
                .Returns(callInfo => new ShutdownCommand(
                    powerController,
                    new ShutdownArgs(
                        (DateTime)callInfo[0],
                        true,
                        false)));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True(599 <= powerController.SecondsTillShutdown &&
                        powerController.SecondsTillShutdown <= 600);
        }

        [Fact]
        public void ShutdownShouldOccurAtTheScheduledTimeWhenMoreThan10MinutesTillShutdown()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.GetAll()
                .Returns(new[]
                {
                    d.AddMinutes(10).AddSeconds(6),
                    d.AddMinutes(10).AddSeconds(8)
                });

            shutdownCommandFactory.CreateDailyShutdownCommand(Arg.Any<DateTime>())
                .Returns(callInfo => new ShutdownCommand(
                    powerController,
                    new ShutdownArgs(
                        (DateTime)callInfo[0],
                        true,
                        false)));

            nightlyShutdownScheduler.ScheduleShutdown();

            Assert.True(605 <= powerController.SecondsTillShutdown &&
                        powerController.SecondsTillShutdown <= 606);
        }
    }
}
