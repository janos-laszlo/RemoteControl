using RemoteControlService.ReceiverDevice.Commands;
using RemoteControlService.ReceiverDevice.MessageReception;
using System;
using System.Diagnostics;

namespace RemoteControlService.ReceiverDevice
{
    class Receiver
    {
        readonly IMessageReceptionist messageReceptioner;
        readonly CommandFactory commandFactory;

        public Receiver()
        {
            messageReceptioner = new MessageReceptionist();
            commandFactory = new CommandFactory();
        }

        public void Start()
        {
            messageReceptioner.MessageReceived += OnMessageReceived;
            messageReceptioner.Start();
            Trace.TraceInformation("The receiver has started.");
        }

        public void Stop()
        {
            messageReceptioner.MessageReceived -= OnMessageReceived;
            messageReceptioner.Stop();
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
