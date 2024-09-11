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
        public RestGlobalCommand RestGlobalCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => throw new NotImplementedException();

        public RestApplicationCommand RestApplicationCommand => throw new NotImplementedException();

        public Task Handle(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }

        public SlashCommandProperties Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
