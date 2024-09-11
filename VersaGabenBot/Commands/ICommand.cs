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
    internal interface ICommand
    {
        public string Name { get; }
        public RestApplicationCommand RestApplicationCommand { get; }
        public SlashCommandProperties Prepare();
        public Task Handle(SocketSlashCommand command);
    }
}
