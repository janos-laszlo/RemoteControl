using System;
using System.Diagnostics;
using RemoteControlService.NightlyShutdown;
using RemoteControlService.MessageReception;
using RemoteControlService.Commands;
using RemoteControlService.Commands.CommandFactories;

namespace RemoteControlService
{
    public class Receiver
    {
        private readonly IMessageReceptionist messageReceptionist;
        private readonly JsonCommandFactory commandFactory;
        private readonly IShutdownScheduler nightlyShutdownScheduler;

        public Receiver(IMessageReceptionist messageReceptionist,
                        JsonCommandFactory commandFactory,
                        IShutdownScheduler nightlyShutdownScheduler)
        {
            this.messageReceptionist = messageReceptionist;
            this.commandFactory = commandFactory;
            this.nightlyShutdownScheduler = nightlyShutdownScheduler;
        }

        public void Start()
        {
            messageReceptionist.MessageReceived += OnMessageReceived;
            messageReceptionist.Start();
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
