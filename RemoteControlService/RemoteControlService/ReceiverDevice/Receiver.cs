using RemoteControlService.ReceiverDevice.Commands;
using RemoteControlService.ReceiverDevice.DailyShutdown;
using RemoteControlService.ReceiverDevice.MessageReception;
using System;
using System.Diagnostics;

namespace RemoteControlService.ReceiverDevice
{
    public class Receiver
    {
        private readonly IMessageReceptionist messageReceptionist;
        private readonly CommandFactory commandFactory;
        private readonly IDailyShutdownScheduler dailyShutodwnScheduler;

        public Receiver(IMessageReceptionist messageReceptionist,
                        CommandFactory commandFactory,
                        IDailyShutdownScheduler dailyShutodwnScheduler)
        {
            this.messageReceptionist = messageReceptionist;
            this.commandFactory = commandFactory;
            this.dailyShutodwnScheduler = dailyShutodwnScheduler;
        }

        public void Start()
        {
            messageReceptionist.MessageReceived += OnMessageReceived;
            messageReceptionist.Start();
            _ = dailyShutodwnScheduler.ScheduleDailyShutdown();
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
