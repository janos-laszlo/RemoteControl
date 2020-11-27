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
    // Improvements: delegate the 2 identified responsibilites
    public class NightlyShutdownScheduler : IShutdownScheduler
    {
        private const int HISTORY_MAX_SIZE = 5;
        private readonly TimeSpan MIN_TIME = new TimeSpan(22, 0, 0);
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;
        private readonly IPowerController powerController;
        private readonly ISystemInformation systemInformation;
        private readonly IShutdownCalculator shutdownCalculator;

        public NightlyShutdownScheduler(IShutdownHistoryStorage shutdownHistoryStorage,
                                      IPowerController powerController,
                                      ISystemInformation systemInformation,
                                      IShutdownCalculator shutdownCalculator)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.powerController = powerController;
            this.systemInformation = systemInformation;
            this.shutdownCalculator = shutdownCalculator;
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

            TimeSpan shutdownHourAndMinute = shutdownCalculator.CalculateShutdownTime(shutdownHistory.Select(d => new TimeSpan(d.Hour, d.Minute, d.Second)), MIN_TIME);
            DateTime nextShutdown = GetNextClosestShutdownDateTime(shutdownHourAndMinute);

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

        private static DateTime GetNextClosestShutdownDateTime(TimeSpan shutdownTime)
        {
            var nextClosestShutdownDatetime = new DateTime(DateTime.Now.Year,
                                                           DateTime.Now.Month,
                                                           DateTime.Now.Day,
                                                           shutdownTime.Hours,
                                                           shutdownTime.Minutes,
                                                           shutdownTime.Seconds);

            if (nextClosestShutdownDatetime < DateTime.Now)
            {
                nextClosestShutdownDatetime = nextClosestShutdownDatetime.AddDays(1);
            }

            return nextClosestShutdownDatetime;
        }
    }
}
