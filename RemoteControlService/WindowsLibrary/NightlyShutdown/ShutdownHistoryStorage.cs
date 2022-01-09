using Domain;
using Domain.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsLibrary.Utils;

namespace WindowsLibrary.NightlyShutdown
{
    public class ShutdownHistoryStorage : IShutdownHistoryStorage
    {
        private readonly HashSet<DateTime> dateTimes;
        private readonly Locations locations;

        public ShutdownHistoryStorage(Locations locations)
        {
            if (!File.Exists(locations.ShutdownHistoryFilePath))
            {
                // Empty json array.
                File.WriteAllText(locations.ShutdownHistoryFilePath, "[]");
            }

            dateTimes = JSONUtils.FromJson<HashSet<DateTime>>(
                File.ReadAllText(locations.ShutdownHistoryFilePath));
            this.locations = locations;
        }

        public IEnumerable<DateTime> GetAll(Func<DateTime, bool> filter = null)
        {
            if (filter != null)
            {
                return dateTimes.Where(filter).ToList();
            }

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
            File.WriteAllText(locations.ShutdownHistoryFilePath, JSONUtils.ToJson(dateTimes));
        }
    }
}
