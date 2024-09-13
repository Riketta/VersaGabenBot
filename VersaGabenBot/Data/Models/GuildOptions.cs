using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Models
{
    internal class GuildOptions
    {
        public ulong GuildID { get; set; }
        public uint MessageHistoryLimitPerChannel { get; set; } = 300;

        private GuildOptions() { }

        public GuildOptions(ulong guildId)
        {
            GuildID = guildId;
        }
    }
}
