using Domain.CommandFactories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domain.NightlyShutdown
{
    // Responsibility:
    // -schedules next nightly shutdown
    // --------------------------------
    // get shutdowns
    // calculate next shutdown based previous shutdowns
    // schedule next shutdown based on the calculation
    public class NightlyShutdownScheduler : IShutdownScheduler
    {
        private readonly IShutdownHistoryStorage shutdownHistoryStorage;
        private readonly IShutdownCalculator nightlyShutdownCalculator;
        private readonly IShutdownCommandFactory shutdownCommandFactory;
        private readonly ILogger<NightlyShutdownScheduler> logger;

        public NightlyShutdownScheduler(
            IShutdownHistoryStorage shutdownHistoryStorage,
            IShutdownCalculator nightlyShutdownCalculator,
            IShutdownCommandFactory shutdownCommandFactory,
            ILogger<NightlyShutdownScheduler> logger)
        {
            this.shutdownHistoryStorage = shutdownHistoryStorage;
            this.nightlyShutdownCalculator = nightlyShutdownCalculator;
            this.shutdownCommandFactory = shutdownCommandFactory;
            this.logger = logger;
        }

        public void ScheduleShutdown()
        {
            IEnumerable<DateTime> shutdownHistory = shutdownHistoryStorage.GetAll();
            if (!shutdownHistory.Any())
            {
                logger.LogInformation("shutdown history is empty");
                return;
            }

            DateTime nextForecastedShutdownTime = nightlyShutdownCalculator.GetNextShutdown(shutdownHistory);
            DateTime nextShutdownTime = AddBuffer(nextForecastedShutdownTime, minutes: 10);
            ExecuteDailyShutdownAt(nextShutdownTime);
            logger.LogInformation($"Shutdown was scheduled to happen at: {nextShutdownTime}");
        }

        private DateTime AddBuffer(DateTime nextShutdownTime, int minutes)
        {
            DateTime now = DateTime.Now;
            return nextShutdownTime.AddMinutes(-minutes) > now ?
                nextShutdownTime : now.AddMinutes(minutes);
        }

        private void ExecuteDailyShutdownAt(DateTime nextShutdownTime)
        {
            shutdownCommandFactory.CreateDailyShutdownCommand(nextShutdownTime).Execute();
        }

        public void CancelShutdown()
        {
            shutdownCommandFactory.CreateCancelShutdownCommand().Execute();
        }
    }
}
