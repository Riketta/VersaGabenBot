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
        public int DatabaseSaveInterval { get; set; } = 5 * 60 * 1000;
        public uint DefaultMessageHistoryLimitPerChannel { get; set; } = 100;
    }
}
