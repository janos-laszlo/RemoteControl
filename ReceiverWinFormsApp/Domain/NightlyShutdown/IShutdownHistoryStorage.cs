using System;
using System.Collections.Generic;

namespace Domain.NightlyShutdown
{
    public interface IShutdownHistoryStorage
    {
        void Add(DateTime dateTime);
        IEnumerable<DateTime> GetAll(Func<DateTime, bool>? filter = default);
        void Remove(DateTime dateTime);
    }
}
