using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    internal class LlmOptionsGlobal : IOptions
    {
        public bool IncludeMessageSender { get; set; } = false;
        public string SenderPlaceholder { get; set; } = "{sender}";
        public string MessagePlaceholder { get; set; } = "{message}";
        public string MessageWithSenderTemplate { get; set; } = "{sender}: {message}";
        public bool RemoveEmptyLines { get; set; } = false;
        public int MaxEmptyLines { get; set; } = 1;
    }
}
