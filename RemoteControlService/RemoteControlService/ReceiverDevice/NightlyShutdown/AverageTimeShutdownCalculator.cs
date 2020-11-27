using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public class AverageTimeShutdownCalculator : IShutdownCalculator
    {
        public TimeSpan CalculateShutdownTime(IEnumerable<TimeSpan> times, TimeSpan minTime)
        {
            if (times == null || !times.Any()) throw new ArgumentException($"Collection contains no elements.");
            TimeSpan shutdownHourAndMinute = CalculateAverageTimeForTimeInterval(times, minTime);
            return shutdownHourAndMinute;
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
    }
}
