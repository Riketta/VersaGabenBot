using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Models
{
    internal class GuildLlmOptions
    {
        public ulong GuildID { get; set; }
        public uint MessagesContextSize { get; set; } = 100;
        public bool OnlyProcessChatHistoryRelatedToBot { get; set; } = false;
        public double RandomReplyChance { get; set; } = 0.025f;
    }
}
