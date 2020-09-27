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
            IEnumerable<DateTime> shutdownHistory = shutdownHistoryStorage.GetAll();
            if (!shutdownHistory.Any())
            {
                Trace.WriteLine("shutdown history is empty");
                return;
            }

            TimeSpan shutdownHourAndMinute = CalculateAverageShutdownHourFromShudownHistory(shutdownHistory);
            DateTime forecastedShutdownDateTime = GetNextClosestShutdownDatetime(shutdownHourAndMinute);

            var now = DateTime.Now;
            TimeSpan timeTillShutdownWillBeScheduled = TimeSpan.FromTicks(Math.Max((forecastedShutdownDateTime - now - TimeSpan.FromMinutes(10)).Ticks, TimeSpan.Zero.Ticks));
            DateTime actualShutdownDatetime = now.Add(timeTillShutdownWillBeScheduled).AddMinutes(10);
            actualShutdownDatetime = actualShutdownDatetime.AddSeconds(-actualShutdownDatetime.Second);
            Trace.WriteLine($"Shutdown was scheduled to happen at: {actualShutdownDatetime}");
            await Task.Delay(timeTillShutdownWillBeScheduled);
            powerController.ScheduleShutdown(seconds: (int)(actualShutdownDatetime - DateTime.Now).TotalSeconds);
        }

        public void UpdateShutdownHistory()
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

        private TimeSpan CalculateAverageShutdownHourFromShudownHistory(IEnumerable<DateTime> shutdownHistory)
        {
            var sortedShutdownHistory = new SortedSet<DateTime>(shutdownHistory);
            TimeSpan average = TimeSpan.FromSeconds(0);
            for (int i = 1; i < sortedShutdownHistory.Count; i++)
            {
                average += TimeSpan.FromTicks((sortedShutdownHistory.ElementAt(i) - sortedShutdownHistory.ElementAt(0)).Ticks % TimeSpan.TicksPerDay);
            }

            average = TimeSpan.FromTicks(average.Ticks / sortedShutdownHistory.Count);

            return TimeSpan.FromTicks((average.Ticks
                                       + TimeSpan.FromHours(sortedShutdownHistory.ElementAt(0).Hour).Ticks
                                       + TimeSpan.FromMinutes(sortedShutdownHistory.ElementAt(0).Minute).Ticks) 
                                       % TimeSpan.TicksPerDay);
        }

        private DateTime GetNextClosestShutdownDatetime(TimeSpan shutdownHour)
        {
            var nextClosestShutdownDatetime = new DateTime(DateTime.Now.Year,
                                                           DateTime.Now.Month,
                                                           DateTime.Now.Day,
                                                           shutdownHour.Hours,
                                                           shutdownHour.Minutes,
                                                           0);

            if (nextClosestShutdownDatetime < DateTime.Now)
            {
                nextClosestShutdownDatetime = nextClosestShutdownDatetime.AddDays(1);
            }

            return nextClosestShutdownDatetime;
        }
    }
}
