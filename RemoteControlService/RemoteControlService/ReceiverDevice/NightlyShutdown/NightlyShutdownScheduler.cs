using RemoteControlService.ReceiverDevice.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
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
        private readonly IPowerController powerController;
        private readonly ISystemInformation systemInformation;
        private readonly IShutdownCalculator nightlyShutdownCalculator;

        public NightlyShutdownScheduler(IShutdownHistoryStorage shutdownHistoryStorage,
                                      IPowerController powerController,
                                      ISystemInformation systemInformation,
                                      IShutdownCalculator nightlyShutdownCalculator)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.powerController = powerController;
            this.systemInformation = systemInformation;
            this.nightlyShutdownCalculator = nightlyShutdownCalculator;
        }

        public async Task ScheduleShutdown()
        {
            UpdateShutdownHistory();
            IEnumerable<DateTime> shutdownHistory = shutdownHistoryStorage.GetAll();
            if (!shutdownHistory.Any())
            {
                Trace.WriteLine("shutdown history is empty");
                return;
            }

            DateTime nextShutdown = nightlyShutdownCalculator.GetNextShutdown(shutdownHistory);

            var now = DateTime.Now;
            TimeSpan timeTillShutdownWillBeScheduled = TimeSpan.FromTicks(Math.Max((nextShutdown - now - TimeSpan.FromMinutes(10)).Ticks, TimeSpan.Zero.Ticks));
            DateTime actualShutdownDatetime = now.Add(timeTillShutdownWillBeScheduled).AddMinutes(10);
            Trace.TraceInformation($"Shutdown was scheduled to happen at: {actualShutdownDatetime}");
            await Task.Delay(timeTillShutdownWillBeScheduled);
            powerController.ScheduleShutdown(seconds: (int)(actualShutdownDatetime - DateTime.Now).TotalSeconds, overrideScheduledShutdown: false);
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
