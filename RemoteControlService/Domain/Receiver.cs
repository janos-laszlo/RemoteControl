using Domain.Common.TaskScheduling;
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
        private readonly ITaskScheduler taskScheduler;
        private readonly ILogger<Receiver> logger;

        public Receiver(
            CommandProcessor commandProcessor,
            IShutdownHistoryUpdater shutdownHistoryUpdater,
            IShutdownScheduler nightlyShutdownScheduler,
            ITaskScheduler taskScheduler,
            ILogger<Receiver> logger)
        {
            this.commandProcessor = commandProcessor;
            this.shutdownHistoryUpdater = shutdownHistoryUpdater;
            this.nightlyShutdownScheduler = nightlyShutdownScheduler;
            this.taskScheduler = taskScheduler;
            this.logger = logger;
        }

        public void Start()
        {
            taskScheduler.Start();
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
            taskScheduler.Stop();
            logger.LogInformation("The receiver has stopped.");
        }
    }
}
