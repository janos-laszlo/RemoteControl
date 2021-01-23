using Domain.NightlyShutdown;
using System.Diagnostics;

namespace RemoteControlService
{
    class Receiver
    {
        private readonly CommandProcessor commandProcessor;
        private readonly IShutdownHistoryUpdater shutdownHistoryUpdater;
        private readonly IShutdownScheduler nightlyShutdownScheduler;

        public Receiver(CommandProcessor commandProcessor,
                        IShutdownHistoryUpdater shutdownHistoryUpdater,
                        IShutdownScheduler nightlyShutdownScheduler)
        {
            this.commandProcessor = commandProcessor;
            this.shutdownHistoryUpdater = shutdownHistoryUpdater;
            this.nightlyShutdownScheduler = nightlyShutdownScheduler;
        }

        public void Start()
        {
            commandProcessor.Start();
            shutdownHistoryUpdater.UpdateShutdownHistory();
            nightlyShutdownScheduler.ScheduleShutdown();
            Trace.WriteLine("The receiver has started.");
        }

        public void Stop()
        {
            commandProcessor.Stop();
            nightlyShutdownScheduler.CancelShutdown();
            Trace.WriteLine("The receiver has stopped.");
        }
    }
}
