using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    internal class LlmOptions : IOptions
    {
        public bool OnlySaveChatHistoryRelatedToBot { get; set; } = false;
        public double RandomReplyChance { get; set; } = 0.01f;
    }
}
