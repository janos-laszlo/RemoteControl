using Domain.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class NightlyShutdownCalculatorTest
    {
        private readonly NightlyShutdownCalculator nightlyShutdownCalculator;

        public NightlyShutdownCalculatorTest()
        {
            nightlyShutdownCalculator = new NightlyShutdownCalculator();
        }

        [Fact]
        public void CalculateShutdownTime_NullInput_ArgumentException()
        {
            var times = Enumerable.Empty<DateTime>();
            Assert.Throws<ArgumentException>(
                () => nightlyShutdownCalculator.GetNextShutdown(times));
        }

        [Fact]
        public void CalculateShutdownTime_EmptyCollection_ArgumentException()
        {
            IEnumerable<DateTime> times = Enumerable.Empty<DateTime>();
            Assert.Throws<ArgumentException>(
                () => nightlyShutdownCalculator.GetNextShutdown(times));
        }

        [Fact]
        public void CalculateNextShutdown_AllTimesGreaterThanMinTime()
        {
            var times = new DateTime[]
            {
                new DateTime(2020, 11, 27, 22,0,0),
                new DateTime(2020, 11, 27, 23,0,0),
                new DateTime(2020, 11, 27, 23,0,0),
                new DateTime(2020, 11, 27, 22,0,0),
            };

            DateTime result = nightlyShutdownCalculator.GetNextShutdown(times);

            var now = DateTime.Now;
            DateTime expected = new(now.Year, now.Month, now.Day, 22, 30, 0);
            Assert.True(expected == result || expected.AddDays(1) == result);
        }

        [Fact]
        public void CalculateShutdownTime_AllTimesLessThanMinTime()
        {
            var times = new DateTime[]
            {
                new DateTime(2020, 11, 27, 2,0,0),
                new DateTime(2020, 11, 27, 3,0,0),
                new DateTime(2020, 11, 27, 4, 0,0),
                new DateTime(2020, 11, 27, 5,0,0),
                new DateTime(2020, 11, 27, 6,0,0),
            };

            DateTime result = nightlyShutdownCalculator.GetNextShutdown(times);

            var now = DateTime.Now;
            DateTime expected = new(now.Year, now.Month, now.Day, 4, 0, 0);
            Assert.True(expected == result || expected.AddDays(1) == result);
        }

        [Fact]
        public void CalculateShutdownTime_AllTimesGreaterAndLessThanMinTime()
        {
            var shutdownHistory = new DateTime[]
            {
                new DateTime(2020, 11, 27, 22,0,0),
                new DateTime(2020, 11, 27, 23,0,0),
                new DateTime(2020, 11, 27, 0, 0,0),
                new DateTime(2020, 11, 27, 1,0,0),
                new DateTime(2020, 11, 27, 2,0,0),
            };

            DateTime result = nightlyShutdownCalculator.GetNextShutdown(shutdownHistory);

            var now = DateTime.Now;
            DateTime expected = new(now.Year, now.Month, now.Day, 0, 0, 0);
            Assert.True(expected == result || expected.AddDays(1) == result);
        }
    }
}
