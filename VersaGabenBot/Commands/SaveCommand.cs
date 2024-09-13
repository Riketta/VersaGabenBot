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
    internal class SaveCommand : IGlobalCommand
    {
        public string Name => "save";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Force save the current state of the bot.")
                .Build();

            return command;
        }

        public Task Handle(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
