using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Guilds;

namespace VersaGabenBot.Options
{
    internal class GuildOptions : IOptions
    {
        public HashSet<Guild> Guilds { get; set; } = new HashSet<Guild>();
        public uint DefaultMessageHistoryLimitPerChannel { get; private set; } = 100;
    }
}
