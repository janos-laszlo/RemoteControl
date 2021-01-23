﻿using Domain.CommandFactories;
using Domain.Commands;
using System;
using System.Collections.Generic;
using WindowsLibrary.Builders;
using WindowsLibrary.Controllers;
using WindowsLibrary.DTOs;
using WindowsLibrary.Utils;

namespace WindowsLibrary.CommandFactories
{
    public class JsonCommandFactory : ITextCommandFactory
    {
        static readonly Dictionary<string, Func<string, ICommand>> CommandTypes =
            new Dictionary<string, Func<string, ICommand>>
        {
            {
                nameof(ShutdownCommand),
                (json) =>  new ShutdownCommand(
                    new CmdLinePowerController(),
                    new WindowsShutdownCommandArgumentsBuilder()
                        .WithSeconds(Create<ShutdownCommandDTO>(json).Seconds)
                        .ShouldOverrideExistingShutdown(true)
                        .ShouldShowNotification(true))
            },
            {
                nameof(CancelShutdownCommand),
                (json) => new CancelShutdownCommand(
                    new CmdLinePowerController())
            },
            {
                nameof(SetVolumeCommand),
                (json) => new SetVolumeCommand(
                    Create<SetVolumeCommandDTO>(json).Percent,
                    new CmdLineVolumeController())
            },
            {
                nameof(HibernateCommand),
                (json) => new HibernateCommand(
                    new CmdLinePowerController())
            }
        };

        public ICommand Create(string unparsedCmd)
        {
            var cmd = JSONUtils.FromJson<CommandDTO>(unparsedCmd);

            if (!CommandTypes.ContainsKey(cmd.Name))
                throw new Exception($"Command type {cmd.Name} is not handled");

            return CommandTypes[cmd.Name](cmd.Command);
        }

        private static T Create<T>(string json)
        {
            return JSONUtils.FromJson<T>(json);
        }
    }
}
