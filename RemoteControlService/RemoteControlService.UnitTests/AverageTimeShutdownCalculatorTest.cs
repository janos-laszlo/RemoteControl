using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlService.ReceiverDevice.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class AverageTimeShutdownCalculatorTest
    {
        AverageTimeShutdownCalculator averageTimeShutdownCalculator;

        public AverageTimeShutdownCalculatorTest()
        {
            averageTimeShutdownCalculator = new AverageTimeShutdownCalculator();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateShutdownTime_NullInput_ArgumentException()
        {
            IEnumerable<TimeSpan> times = null;
            averageTimeShutdownCalculator.CalculateShutdownTime(times, TimeSpan.FromHours(22));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateShutdownTime_EmptyCollection_ArgumentException()
        {
            IEnumerable<TimeSpan> times = Enumerable.Empty<TimeSpan>();
            averageTimeShutdownCalculator.CalculateShutdownTime(times, TimeSpan.FromHours(22));
        }

        [TestMethod]
        public void CalculateNextShutdown_AllTimesGreaterThanMinTime()
        {
            var minTime = new TimeSpan(1, 0, 0);
            var times = new TimeSpan[]
            {
                new TimeSpan(2,0,0),
                new TimeSpan(3,0,0),
                new TimeSpan(4,0,0),
                new TimeSpan(5,0,0),
                new TimeSpan(6,0,0),
            };

            TimeSpan result = averageTimeShutdownCalculator.CalculateShutdownTime(times, minTime);

            Assert.AreEqual(new TimeSpan(4, 0, 0), result);
        }

        [TestMethod]
        public void CalculateShutdownTime_AllTimesLessThanMinTime()
        {
            var minTime = new TimeSpan(10, 0, 0);
            var times = new TimeSpan[]
            {
                new TimeSpan(2,0,0),
                new TimeSpan(3,0,0),
                new TimeSpan(4,0,0),
                new TimeSpan(5,0,0),
                new TimeSpan(6,0,0),
            };

            TimeSpan result = averageTimeShutdownCalculator.CalculateShutdownTime(times, minTime);

            Assert.AreEqual(new TimeSpan(4, 0, 0), result);
        }

        [TestMethod]
        public void CalculateShutdownTime_AllTimesGreaterAndLessThanMinTime()
        {
            var minTime = new TimeSpan(22, 0, 0);
            var times = new TimeSpan[]
            {
                new TimeSpan(23,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(1,0,0),
                new TimeSpan(22,0,0),
                new TimeSpan(2,0,0)
            };

            TimeSpan result = averageTimeShutdownCalculator.CalculateShutdownTime(times, minTime);

            Assert.AreEqual(new TimeSpan(0, 0, 0), result);
        }
    }
}
