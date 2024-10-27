using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    internal class LlmOptions : IOptions
    {
        public string DefaultSystemPrompt { get; set; } = "You are a funny public chat bot. Provide very brief, concise responses. You can be toxic.";
        public bool IncludeMessageSender { get; set; } = false;
        public string SenderPlaceholder { get; set; } = "{sender}";
        public string MessagePlaceholder { get; set; } = "{message}";
        public string MessageWithSenderTemplate { get; set; } = "{sender}: {message}";
        public bool RemoveEmptyLines { get; set; } = false;
        public int MaxEmptyLines { get; set; } = 1;
    }
}
