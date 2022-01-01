using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.MessageReception;
using System;
using System.Diagnostics;

namespace Domain
{
    public class CommandProcessor
    {
        private readonly IMessageReceptionist messageReceptionist;
        private readonly ITextCommandFactory commandFactory;

        public CommandProcessor(
            IMessageReceptionist messageReceptionist,
            ITextCommandFactory commandFactory)
        {
            this.messageReceptionist = messageReceptionist;
            this.commandFactory = commandFactory;
        }

        public void Start()
        {
            messageReceptionist.MessageProcessor = TryParsingAndExecutingCommand;
            messageReceptionist.Start();
        }

        public void Stop()
        {
            messageReceptionist.Stop();

        }

        private Maybe<string> TryParsingAndExecutingCommand(string cmd)
        {
            try
            {
                return ParseAndExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                Trace.TraceError($"Failed to parse and execute request: {e}");
                return Maybe<string>.None();
            }
        }

        private Maybe<string> ParseAndExecuteCommand(string cmd)
        {
            ICommand command = commandFactory.Create(cmd);
            return command.Execute();
        }
    }
}
