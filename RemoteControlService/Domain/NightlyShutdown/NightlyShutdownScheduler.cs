using Domain.Commands;
using Domain.Common.TaskScheduling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domain.NightlyShutdown
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
        private ScheduledTask shutdownTask;

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
            shutdownTask = CreateShutdownScheduledTask(nextShutdown.AddMinutes(-10));
            taskScheduler.ScheduleTask(shutdownTask);
            Trace.TraceInformation($"Shutdown was scheduled to happen at: {nextShutdown}");
        }

        public void CancelShutdown()
        {
            shutdownTask.Cancel();
        }

        private ScheduledTask CreateShutdownScheduledTask(DateTime executeAt)
        {
            return new ScheduledTask(shutdownTask, executeAt);

            void shutdownTask() => shutdownCommandFactory.CreateShutdownCommand(
                seconds: 600,
                overrideScheduledShutdown: false).Execute();
        }
    }
}
