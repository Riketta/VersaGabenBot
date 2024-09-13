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
    internal class ClearCommand : IGlobalCommand
    {
        public string Name => "clear";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Clear LLM context for current channel.")
                .Build();

            return command;
        }

        public Task Handle(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
