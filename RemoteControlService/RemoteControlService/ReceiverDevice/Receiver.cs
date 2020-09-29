using RemoteControlService.ReceiverDevice.Commands;
using RemoteControlService.ReceiverDevice.Controllers;
using RemoteControlService.ReceiverDevice.DailyShutdown;
using RemoteControlService.ReceiverDevice.MessageReception;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice
{
    class Receiver
    {
        private readonly IMessageReceptionist messageReceptioner;
        private readonly CommandFactory commandFactory;
        private readonly DailyShutdownScheduler dailyShutodwnScheduler;

        public Receiver()
        {
            messageReceptioner = new MessageReceptionist();
            commandFactory = new CommandFactory();
            dailyShutodwnScheduler = new DailyShutdownScheduler(new ShutdownHistoryStorage(),
                                                                new CmdLinePowerController(),
                                                                new SystemInformation());
        }

        public void Start()
        {
            messageReceptioner.MessageReceived += OnMessageReceived;
            messageReceptioner.Start();
            _ = dailyShutodwnScheduler.ScheduleDailyShutdown();
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
