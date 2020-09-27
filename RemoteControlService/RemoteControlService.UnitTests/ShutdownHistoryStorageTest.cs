using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlService.ReceiverDevice.DailyShutdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class ShutdownHistoryStorageTest
    {
        private IShutdownHistoryStorage shutdownHistoryStorage;

        [TestCleanup]
        public void CleanupTest()
        {
            File.Delete(ShutdownHistoryStorage.SHUTDOWN_HISTORY_FILE);
        }

        [TestInitialize]
        public void Init()
        {
            shutdownHistoryStorage = new ShutdownHistoryStorage();
            shutdownHistoryStorage.Add(DateTime.Now);
            shutdownHistoryStorage.Add(DateTime.UtcNow);
            shutdownHistoryStorage.Add(DateTime.UtcNow.AddDays(1));
        }

        [TestMethod]
        public void GetAll_WhenDatetimesInStorage_ThenReturnedWithCorrectValues()
        {
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();

            Assert.IsTrue(times.Count() == 3);
        }

        [TestMethod]
        public void Add_WhenNewDatetimesAdded_ThenStorageIncreaseExpected()
        {
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();
            Assert.IsTrue(times.Count() == 3);
        }

        [TestMethod]
        public void Add_WhenDatetimesRemoved_ThenStorageDecreaseExpected()
        {
            var d = DateTime.Now;
            shutdownHistoryStorage.Add(d.AddDays(2));
            shutdownHistoryStorage.Add(d.AddDays(12));
            shutdownHistoryStorage.Add(d.AddDays(3));
            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll();

            shutdownHistoryStorage.Remove(d.AddDays(2));
            shutdownHistoryStorage.Remove(d.AddDays(12));
            Assert.IsTrue(times.Count() == 4);
        }
    }
}
