﻿using Domain.CommandFactories;
using Domain.NightlyShutdown;
using RemoteControlService.UniTests.Mocks;
using System;
using System.IO;
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

        public NightlyShutdownSchedulerTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            powerController = new CmdLinePowerControllerMock();
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownCalculator = new NightlyShutdownCalculator();
            shutdownCommandFactory = new ParameterizedShutdownCommandFactory(
                powerController);
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
