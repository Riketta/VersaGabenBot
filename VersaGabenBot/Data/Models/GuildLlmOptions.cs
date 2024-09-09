using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Models
{
    internal class GuildLlmOptions : IOptions
    {
        public bool OnlySaveChatHistoryRelatedToBot { get; set; } = false;
        public double RandomReplyChance { get; set; } = 0.01f;
    }
}
