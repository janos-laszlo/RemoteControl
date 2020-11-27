using System;
using System.Collections.Generic;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public interface IShutdownCalculator
    {
        /// <summary>
        /// Returns the average of the <paramref name="times"/> having as base time <paramref name="minTime"/>.
        /// <para>Ex: <paramref name="times"/> = [23:00, 01:00] and <paramref name="minTime"/> = 21:00 should output 00:00 not 12:00 </para>
        /// </summary>
        /// <param name="times">A collection of <see cref="System.TimeSpan"/></param>.
        /// <param name="minTime">The base time. All <see cref="System.TimeSpan"/> in <paramref name="times"/> are considered to be after <paramref name="minTime"/>.</param>
        TimeSpan CalculateShutdownTime(IEnumerable<TimeSpan> times, TimeSpan minTime);
    }
}
