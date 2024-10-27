using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Data.Models
{
    internal class Guild
    {
        public ulong GuildID { get; set; }
        public ulong? SystemChannelID { get; set; }
        public GuildOptions Options { get; set; }
    }
}
