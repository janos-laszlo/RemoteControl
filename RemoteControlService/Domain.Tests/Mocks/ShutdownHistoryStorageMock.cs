using Domain.NightlyShutdown;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Tests.Mocks
{
    class ShutdownHistoryStorageMock : IShutdownHistoryStorage
    {
        private readonly HashSet<DateTime> dateTimes = new HashSet<DateTime>();

        public void Add(DateTime dateTime)
        {
            dateTimes.Add(dateTime);
        }

        public IEnumerable<DateTime> GetAll(Func<DateTime, bool> filter = null)
        {
            if (filter != null)
            {
                return dateTimes.Where(filter).ToList();
            }

            return dateTimes;
        }

        public void Remove(DateTime dateTime)
        {
            dateTimes.Remove(dateTime);
        }
    }
}
