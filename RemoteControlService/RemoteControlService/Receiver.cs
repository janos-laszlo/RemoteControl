using Domain.CommandFactories;
using Domain.Commands;
using Domain.MessageReception;
using Domain.NightlyShutdown;
using System;
using System.Diagnostics;

namespace RemoteControlService
{
    public class Receiver
    {
        private readonly IMessageReceptionist messageReceptionist;
        private readonly ITextCommandFactory commandFactory;
        private readonly IShutdownHistoryUpdater shutdownHistoryUpdater;
        private readonly IShutdownScheduler nightlyShutdownScheduler;

        public Receiver(IMessageReceptionist messageReceptionist,
                        ITextCommandFactory commandFactory,
                        IShutdownHistoryUpdater shutdownHistoryUpdater,
                        IShutdownScheduler nightlyShutdownScheduler)
        {
            this.messageReceptionist = messageReceptionist;
            this.commandFactory = commandFactory;
            this.shutdownHistoryUpdater = shutdownHistoryUpdater;
            this.nightlyShutdownScheduler = nightlyShutdownScheduler;
        }

        public void Start()
        {
            messageReceptionist.MessageReceived += OnMessageReceived;
            messageReceptionist.Start();
            shutdownHistoryUpdater.UpdateShutdownHistory();
            nightlyShutdownScheduler.ScheduleShutdown();
            Trace.WriteLine("The receiver has started.");
        }

        public void Stop()
        {
            messageReceptionist.MessageReceived -= OnMessageReceived;
            messageReceptionist.Stop();
            Trace.WriteLine("The receiver has stopped.");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
        {
            TryParsingAndExecutingCommand(eventArgs.Message);
        }

        private void TryParsingAndExecutingCommand(string cmd)
        {
            try
            {
                ParseAndExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                Trace.TraceError($"Failed to parse and execute command: {e}");
            }
        }

        private void ParseAndExecuteCommand(string cmd)
        {
            ICommand command = commandFactory.Create(cmd);
            command.Execute();
        }
    }
}
