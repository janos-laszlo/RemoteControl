using Domain.NightlyShutdown;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Domain
{
    public class Receiver
    {
        private readonly CommandProcessor commandProcessor;
        private readonly IShutdownHistoryUpdater shutdownHistoryUpdater;
        private readonly IShutdownScheduler nightlyShutdownScheduler;
        private readonly ILogger<Receiver> logger;

        public Receiver(
            CommandProcessor commandProcessor,
            IShutdownHistoryUpdater shutdownHistoryUpdater,
            IShutdownScheduler nightlyShutdownScheduler,
            ILogger<Receiver> logger)
        {
            this.commandProcessor = commandProcessor;
            this.shutdownHistoryUpdater = shutdownHistoryUpdater;
            this.nightlyShutdownScheduler = nightlyShutdownScheduler;
            this.logger = logger;
        }

        public void Start()
        {
            commandProcessor.Start();
            shutdownHistoryUpdater.UpdateShutdownHistory();
            Task.Run(async () =>
            {
                // Wait for Windows Task Scheduler to start up.
                await Task.Delay(10000);
                nightlyShutdownScheduler.ScheduleShutdown();
            });

            logger.LogInformation("The receiver has started.");
        }

        public void Stop()
        {
            commandProcessor.Stop();
            nightlyShutdownScheduler.CancelShutdown();
            logger.LogInformation("The receiver has stopped.");
        }
    }
}
