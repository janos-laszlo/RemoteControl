using RemoteControlService.Commands;
using RemoteControlService.Common.TaskScheduling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RemoteControlService.NightlyShutdown
{
    // Responsibilities:
    // -schedules next shutdown
    // -updates shutdown history storage
    // get shutdowns
    // calculate next shutdown based previous shutdowns
    // schedule next shutdown based on next calculated shutdown
    // Improvements: delegate the 2 identified responsibilites. Use ShutdownCommand instead of IPowerController
    public class NightlyShutdownScheduler : IShutdownScheduler
    {
        private const int HISTORY_MAX_SIZE = 5;
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;
        private readonly ISystemInformation systemInformation;
        private readonly IShutdownCalculator nightlyShutdownCalculator;
        private readonly ITaskScheduler taskScheduler;
        private readonly IShutdownCommandFactory shutdownCommandFactory;

        public NightlyShutdownScheduler(IShutdownHistoryStorage shutdownHistoryStorage,
                                        ISystemInformation systemInformation,
                                        IShutdownCalculator nightlyShutdownCalculator,
                                        ITaskScheduler taskScheduler,
                                        IShutdownCommandFactory shutdownCommandFactory)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.systemInformation = systemInformation;
            this.nightlyShutdownCalculator = nightlyShutdownCalculator;
            this.taskScheduler = taskScheduler;
            this.shutdownCommandFactory = shutdownCommandFactory;
        }

        public void ScheduleShutdown()
        {
            UpdateShutdownHistory();
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

        private void UpdateShutdownHistory()
        {
            DateTime lastSystemShutdown = systemInformation.GetLastSystemShutdown();
            if (!(22 <= lastSystemShutdown.Hour && lastSystemShutdown.Hour <= 23 ||
                0 <= lastSystemShutdown.Hour && lastSystemShutdown.Hour <= 5))
            {
                return;
            }

            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();
            if (times.Count() >= HISTORY_MAX_SIZE)
            {
                shutdownHistoryStorage.Remove(times.Min());
            }

            shutdownHistoryStorage.Add(lastSystemShutdown);
        }
    }
}
