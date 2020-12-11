using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.NightlyShutdown
{
    public class NightlyShutdownHistoryUpdater : IShutdownHistoryUpdater
    {
        private const int HISTORY_MAX_SIZE = 5;
        private const int BEGINNING_OF_NIGHT = 22;
        private const int END_OF_NIGHT = 6;
        private readonly ISystemInformation systemInformation;
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;

        public NightlyShutdownHistoryUpdater(ISystemInformation systemInformation, IShutdownHistoryStorage shutdownHistoryStorage)
        {
            this.systemInformation = systemInformation;
            this.shutdownHistoryStorage = shutdownHistoryStorage;
        }

        public void UpdateShutdownHistory()
        {
            DateTime lastSystemShutdown = systemInformation.GetLastSystemShutdown();
            if (!IsHourOfNight(lastSystemShutdown))
            {
                return;
            }

            IEnumerable<DateTime> times = shutdownHistoryStorage.GetAll(IsHourOfNight);
            if (times.Count() >= HISTORY_MAX_SIZE)
            {
                shutdownHistoryStorage.Remove(times.Min());
            }

            shutdownHistoryStorage.Add(lastSystemShutdown);
        }

        private static bool IsHourOfNight(DateTime lastSystemShutdown)
        {
            return BEGINNING_OF_NIGHT <= lastSystemShutdown.Hour && lastSystemShutdown.Hour <= END_OF_NIGHT;
        }
    }
}
