using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.NightlyShutdown
{
    public class NightlyShutdownCalculator : IShutdownCalculator
    {
        private static readonly TimeSpan MIN_TIME = new(22, 0, 0);

        /// <summary>
        /// Returns the average of the <paramref name="shutdownHistory"/> having as 
        /// base time 22:00. The times in the collection are considered to be ahead of 22:00.
        /// <para>Ex: <paramref name="shutdownHistory"/> = [23:00, 01:00] 
        /// should output 00:00 not 12:00 </para>
        /// </summary>
        /// <param name="shutdownHistory">A collection of <see cref="DateTime"/>.</param>
        public DateTime GetNextShutdown(IEnumerable<DateTime> shutdownHistory)
        {
            if (!shutdownHistory.Any())
                throw new ArgumentException(
                    $"Collection contains no elements.",
                    nameof(shutdownHistory));
            IEnumerable<TimeSpan> shutdownHistoryTimes =
                shutdownHistory.Select(d => new TimeSpan(d.Hour, d.Minute, d.Second));
            TimeSpan nextShutdownTime = CalculateAverageTimeForTimeInterval(shutdownHistoryTimes);
            return GetNextClosestShutdownDateTime(nextShutdownTime);
        }

        private static TimeSpan CalculateAverageTimeForTimeInterval(IEnumerable<TimeSpan> times)
        {
            long average = 0;
            foreach (var time in times)
            {
                average += GetTicksFromMinTimeTillTime(time) / times.Count();
            }

            return TimeSpan.FromTicks((MIN_TIME.Ticks + average) % TimeSpan.TicksPerDay);
        }

        private static long GetTicksFromMinTimeTillTime(TimeSpan time)
        {
            if (time < MIN_TIME)
            {
                return (TimeSpan.FromDays(1) - MIN_TIME + time).Ticks;
            }

            return (time - MIN_TIME).Ticks;
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
