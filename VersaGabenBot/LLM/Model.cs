using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VersaGabenBot.LLM
{
    class Model
    {
        public string Name { get; set; }
        public bool Multimodal { get; set; } = false;
        public bool Stream { get; set; } = false;
        public string KeepAlive { get; set; } = "-1m";

        public string DefaultSystemPrompt { get; set; } = "You are a funny public chat bot. Provide very brief, concise responses. You can be toxic.";
        public bool IncludeMessageSender { get; set; } = false;
        public string SenderPlaceholder { get; set; } = "{sender}";
        public string MessagePlaceholder { get; set; } = "{message}";
        public string MessageWithSenderTemplate { get; set; } = "{sender}: {message}";
        public bool RemoveEmptyLines { get; set; } = false;
        public int MaxEmptyLines { get; set; } = 1;

        /// <summary>
        /// Details: https://github.com/ollama/ollama/blob/main/docs/modelfile.md.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = [];

        public Model(string name)
        {
            Name = name;
        }
    }
}
