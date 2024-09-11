using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Commands
{
    internal class UnregisterCommand : IGlobalCommand
    {
        public string Name => "unregister";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Register current channel and guild if it is not yet registered.")
                .AddOption("channel", ApplicationCommandOptionType.Channel, "The channel of current guild to be registered.", isRequired: true)
                .Build();

            return command;
        }

        public async Task Handle(SocketSlashCommand command)
        {
        }
    }
}
