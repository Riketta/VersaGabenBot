using Discord.Rest;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Commands
{
    internal class PushMessageCommand : IGlobalCommand
    {
        public string Name => "push";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Push message to channel history.")
                // TODO: for non-authorized users only let self and bot.
                .AddOption("user", ApplicationCommandOptionType.User, "The user to fake message from.", isRequired: true)
                .AddOption("message", ApplicationCommandOptionType.String, "The message content.", isRequired: true)
                .Build();

            return command;
        }

        public Task Handle(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
