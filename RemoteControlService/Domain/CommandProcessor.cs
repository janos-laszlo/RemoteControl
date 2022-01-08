using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.MessageReception;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Domain
{
    public class CommandProcessor
    {
        private readonly IMessageReceptionist messageReceptionist;
        private readonly ITextCommandFactory commandFactory;
        private readonly ILogger<CommandProcessor> logger;

        public CommandProcessor(
            IMessageReceptionist messageReceptionist,
            ITextCommandFactory commandFactory,
            ILogger<CommandProcessor> logger)
        {
            this.messageReceptionist = messageReceptionist;
            this.commandFactory = commandFactory;
            this.logger = logger;
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
                logger.LogError($"Failed to parse and execute request: {e}");
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
