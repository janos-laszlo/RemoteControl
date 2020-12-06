﻿using RemoteControlService.Commands;
using RemoteControlService.Common.TaskScheduling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RemoteControlService.NightlyShutdown
{
    // Responsibility:
    // -schedules next nightly shutdown
    // --------------------------------
    // get shutdowns
    // calculate next shutdown based previous shutdowns
    // schedule next shutdown based on the calculation
    public class NightlyShutdownScheduler : IShutdownScheduler
    {
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;
        private readonly IShutdownCalculator nightlyShutdownCalculator;
        private readonly ITaskScheduler taskScheduler;
        private readonly IShutdownCommandFactory shutdownCommandFactory;

        public NightlyShutdownScheduler(IShutdownHistoryStorage shutdownHistoryStorage,
                                        IShutdownCalculator nightlyShutdownCalculator,
                                        ITaskScheduler taskScheduler,
                                        IShutdownCommandFactory shutdownCommandFactory)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.nightlyShutdownCalculator = nightlyShutdownCalculator;
            this.taskScheduler = taskScheduler;
            this.shutdownCommandFactory = shutdownCommandFactory;
        }

        public void ScheduleShutdown()
        {
            IEnumerable<DateTime> shutdownHistory = shutdownHistoryStorage.GetAll();
            if (!shutdownHistory.Any())
            {
                Trace.WriteLine("shutdown history is empty");
                return;
            }

            DateTime nextShutdown = nightlyShutdownCalculator.GetNextShutdown(shutdownHistory);
            Action shutdownTask = () => shutdownCommandFactory.CreateShutdownCommand(seconds: 600, overrideScheduledShutdown: false).Execute();
            taskScheduler.ScheduleTask(shutdownTask, nextShutdown.AddMinutes(-10));
            Trace.TraceInformation($"Shutdown was scheduled to happen at: {nextShutdown}");
        }
    }
}
