using System;
using System.Collections.Generic;

namespace RemoteControlService.NightlyShutdown
{
    public interface IShutdownHistoryStorage
    {
        void Add(DateTime dateTime);
        IEnumerable<DateTime> GetAll(Func<DateTime, bool> filter = null);
        void Remove(DateTime dateTime);
    }
}
