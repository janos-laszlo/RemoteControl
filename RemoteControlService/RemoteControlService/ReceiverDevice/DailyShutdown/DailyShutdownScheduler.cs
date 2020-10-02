using RemoteControlService.ReceiverDevice.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice.DailyShutdown
{
    public class DailyShutdownScheduler : IDailyShutodwnScheduler
    {
        private const int HISTORY_MAX_SIZE = 5;
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;
        private readonly IPowerController powerController;
        private readonly ISystemInformation systemInformation;
        private readonly TimeSpan MIN_TIME = new TimeSpan(22, 0, 0);

        public DailyShutdownScheduler(IShutdownHistoryStorage shutdownHistoryStorage,
                                      IPowerController powerController,
                                      ISystemInformation systemInformation)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.powerController = powerController;
            this.systemInformation = systemInformation;
        }

        public async Task ScheduleDailyShutdown()
        {
            UpdateShutdownHistory();
            IEnumerable<DateTime> shutdownHistory = shutdownHistoryStorage.GetAll();
            if (!shutdownHistory.Any())
            {
                Trace.WriteLine("shutdown history is empty");
                return;
            }

            TimeSpan shutdownHourAndMinute = CalculateAverageTimeForTimeInterval(shutdownHistory.Select(d => new TimeSpan(d.Hour, d.Minute, d.Second)), MIN_TIME);
            DateTime forecastedShutdownDateTime = GetNextClosestShutdownDatetime(shutdownHourAndMinute);

            var now = DateTime.Now;
            TimeSpan timeTillShutdownWillBeScheduled = TimeSpan.FromTicks(Math.Max((forecastedShutdownDateTime - now - TimeSpan.FromMinutes(10)).Ticks, TimeSpan.Zero.Ticks));
            DateTime actualShutdownDatetime = now.Add(timeTillShutdownWillBeScheduled).AddMinutes(10);
            Trace.WriteLine($"Shutdown was scheduled to happen at: {actualShutdownDatetime}");
            await Task.Delay(timeTillShutdownWillBeScheduled);
            powerController.ScheduleShutdown(seconds: (int)(actualShutdownDatetime - DateTime.Now).TotalSeconds);
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

        public static TimeSpan CalculateAverageTimeForTimeInterval(IEnumerable<TimeSpan> times, TimeSpan minTime)
        {
            long average = 0;
            foreach (var time in times)
            {
                average += GetTicksFromMinTimeTillTime(minTime, time) / times.Count();
            }

            return TimeSpan.FromTicks((minTime.Ticks + average) % TimeSpan.TicksPerDay);
        }

        private static long GetTicksFromMinTimeTillTime(TimeSpan minTime, TimeSpan time)
        {
            if (time < minTime)
            {
                return (TimeSpan.FromDays(1) - minTime + time).Ticks;
            }

            return (time - minTime).Ticks;
        }

        private DateTime GetNextClosestShutdownDatetime(TimeSpan shutdownTime)
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
