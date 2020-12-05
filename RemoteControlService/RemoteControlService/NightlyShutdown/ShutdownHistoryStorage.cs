using RemoteControlService.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemoteControlService.NightlyShutdown
{
    public class ShutdownHistoryStorage : IShutdownHistoryStorage
    {
#if DEBUG
        public const string SHUTDOWN_HISTORY_FILE = "shutdownHistory.json";
#else
        public static readonly string SHUTDOWN_HISTORY_FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shutdownHistory.json");
#endif
        private readonly HashSet<DateTime> dateTimes;

        public ShutdownHistoryStorage()
        {
            if (!File.Exists(SHUTDOWN_HISTORY_FILE))
            {
                // Empty json array.
                File.WriteAllText(SHUTDOWN_HISTORY_FILE, "[]");
            }

            dateTimes = JSONUtils.FromJson<HashSet<DateTime>>(File.ReadAllText(SHUTDOWN_HISTORY_FILE));
        }

        public IEnumerable<DateTime> GetAll()
        {
            return dateTimes;
        }

        public void Add(DateTime dateTime)
        {
            dateTimes.Add(dateTime);
            SaveChanges();
        }

        public void Remove(DateTime dateTime)
        {
            dateTimes.Remove(dateTime);
            SaveChanges();
        }

        private void SaveChanges()
        {
            File.WriteAllText(SHUTDOWN_HISTORY_FILE, JSONUtils.ToJson(dateTimes));
        }
    }
}
