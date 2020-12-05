using RemoteControlService.Commands;
using RemoteControlService.Controllers;
using RemoteControlService.DTOs;
using RemoteControlService.Utils;
using System;
using System.Collections.Generic;

namespace RemoteControlService.Commands.CommandFactories
{
    public class JsonCommandFactory
    {
        static readonly Dictionary<string, Func<string, ICommand>> CommandTypes = new Dictionary<string, Func<string, ICommand>>
        {
            { nameof(ShutdownCommand), (json) =>  new ShutdownCommand(Create<ShutdownCommandDTO>(json).Seconds, new CmdLinePowerController()) },
            { nameof(CancelShutdownCommand), (json) => new CancelShutdownCommand(new CmdLinePowerController()) },
            { nameof(SetVolumeCommand), (json) => new SetVolumeCommand(Create<SetVolumeCommandDTO>(json).Percent, new CmdLineVolumeController()) },
            { nameof(HibernateCommand), (json) => new HibernateCommand(new CmdLinePowerController()) }
        };

        public ICommand Create(string unparsedCmd)
        {
            var cmd = JSONUtils.FromJson<CommandDTO>(unparsedCmd);

            if (!CommandTypes.ContainsKey(cmd.Name)) throw new Exception($"Command type {cmd.Name} is not handled");

            return CommandTypes[cmd.Name](cmd.Command);
        }

        private static T Create<T>(string json)
        {
            return JSONUtils.FromJson<T>(json);
        }
    }
}
