using System;
using System.Collections.Generic;

namespace RemoteControlService.ReceiverDevice.DailyShutdown
{
    public interface IShutdownHistoryStorage
    {
        void Add(DateTime dateTime);
        IEnumerable<DateTime> GetAll();
        void Remove(DateTime dateTime);
    }
}