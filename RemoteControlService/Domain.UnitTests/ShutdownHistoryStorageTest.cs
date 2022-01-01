using Domain.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsLibrary.NightlyShutdown;
using Xunit;

namespace RemoteControlService.UniTests
{
    public class ShutdownHistoryStorageTest
    {
        readonly IShutdownHistoryStorage shutdownHistoryStorage;

        public ShutdownHistoryStorageTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownHistoryStorage.Add(DateTime.Now);
            shutdownHistoryStorage.Add(DateTime.UtcNow);
            shutdownHistoryStorage.Add(DateTime.UtcNow.AddDays(1));
        }

        [Fact]
        public void GetAll_WhenDatetimesInStorage_ThenReturnedWithCorrectValues()
        {
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();

            Assert.Equal(3, times.Count());
        }

        [Fact]
        public void Add_WhenNewDatetimesAdded_ThenStorageIncreaseExpected()
        {
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();
            shutdownHistoryStorage.Add(DateTime.Now.AddMinutes(3));
            Assert.Equal(4, times.Count());
        }

        [Fact]
        public void Add_WhenDatetimesRemoved_ThenStorageDecreaseExpected()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.Add(d.AddDays(2));
            shutdownHistoryStorage.Add(d.AddDays(12));
            shutdownHistoryStorage.Add(d.AddDays(3));
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();

            shutdownHistoryStorage.Remove(d.AddDays(2));
            shutdownHistoryStorage.Remove(d.AddDays(12));
            Assert.Equal(4, times.Count());
        }
    }
}
