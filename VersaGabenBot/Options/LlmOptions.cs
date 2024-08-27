using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    internal class LlmOptions : IOptions
    {
        public bool IncludeMessageSender { get; set; } = false;
        public string SenderPlaceholder { get; set; } = "{sender}";
        public string MessagePlaceholder { get; set; } = "{message}";
        public string MessageWithSenderTemplate { get; set; } = "{sender}: {message}";
        public bool OnlySaveChatHistoryRelatedToBot { get; set; } = false;
        public double RandomReplyChance { get; set; } = 0.01f;
    }
}
