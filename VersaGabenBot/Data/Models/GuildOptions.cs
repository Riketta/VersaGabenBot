using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Models
{
    internal class GuildOptions : IOptions
    {
        public LlmOptionsGuild LlmOptions { get; set; } = new LlmOptionsGuild();
        public uint MessageHistoryLimitPerChannel { get; set; } = 300;
    }
}
