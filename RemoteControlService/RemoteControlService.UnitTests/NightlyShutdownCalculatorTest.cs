using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlService.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class NightlyShutdownCalculatorTest
    {
        private readonly NightlyShutdownCalculator nightlyShutdownCalculator;

        public NightlyShutdownCalculatorTest()
        {
            nightlyShutdownCalculator = new NightlyShutdownCalculator();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateShutdownTime_NullInput_ArgumentException()
        {
            IEnumerable<DateTime> times = null;
            nightlyShutdownCalculator.GetNextShutdown(times);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateShutdownTime_EmptyCollection_ArgumentException()
        {
            IEnumerable<DateTime> times = Enumerable.Empty<DateTime>();
            nightlyShutdownCalculator.GetNextShutdown(times);
        }

        [TestMethod]
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
            DateTime expected = new DateTime(now.Year, now.Month, now.Day, 22, 30, 0);
            Assert.IsTrue(expected == result || expected.AddDays(1) == result);
        }

        [TestMethod]
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
            DateTime expected = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);
            Assert.IsTrue(expected == result || expected.AddDays(1) == result);
        }

        [TestMethod]
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
            DateTime expected = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            Assert.IsTrue(expected == result || expected.AddDays(1) == result);
        }
    }
}
