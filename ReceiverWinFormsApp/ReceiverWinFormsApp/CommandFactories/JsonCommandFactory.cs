﻿using Domain.CommandFactories;
using Domain.Commands;
using Domain.Commands.Arguments;
using Domain.Controllers;
using Microsoft.Extensions.DependencyInjection;
using ReceiverWinFormsApp.DTOs;
using ReceiverWinFormsApp.Utils;
using System;
using System.Collections.Generic;

namespace ReceiverWinFormsApp.CommandFactories
{
    public class JsonCommandFactory : ITextCommandFactory
    {
        private readonly Dictionary<string, Func<string, ICommand>> CommandTypes;

        public JsonCommandFactory(IServiceProvider serviceProvider)
        {
            CommandTypes = new Dictionary<string, Func<string, ICommand>>
            {
                {
                    nameof(ShutdownCommand),
                    json =>  new ShutdownCommand(
                        serviceProvider.GetRequiredService<IPowerController>(),
                        new ShutdownArgs(
                            DateTime.Now.AddSeconds(Create<ShutdownCommandDTO>(json).Seconds),
                            overrideExistingShutdown: true,
                            showNotification: true))
                },
                {
                    nameof(CancelShutdownCommand),
                    json => new CancelShutdownCommand(
                        serviceProvider.GetRequiredService<IPowerController>(),
                        showNotification: true)
                },
                {
                    nameof(SetVolumeCommand),
                    json => new SetVolumeCommand(
                        Create<SetVolumeCommandDTO>(json).Percent,
                        serviceProvider.GetRequiredService<IVolumeController>())
                },
                {
                    nameof(HibernateCommand),
                    json => new HibernateCommand(
                        serviceProvider.GetRequiredService<IPowerController>())
                },
                {
                    nameof(GetNextShutdownCommand),
                    json => new GetNextShutdownCommand(serviceProvider.GetRequiredService<IPowerController>())
                },
                {
                    nameof(GetVolumeCommand),
                    json => new GetVolumeCommand(serviceProvider.GetRequiredService<IVolumeController>())
                },
                {
                    nameof(OpenTaskManagerCommand),
                    json => new OpenTaskManagerCommand(
                        serviceProvider.GetRequiredService<IKeyboardController>())
                }
            };
        }

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
