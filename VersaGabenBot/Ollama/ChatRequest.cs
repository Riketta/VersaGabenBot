using System.Collections.Generic;
using System.Text.Json.Serialization;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Ollama
{
    internal class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<LlmMessage> Messages { get; set; }

        [JsonPropertyName("options")]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("keep_alive")]
        public string KeepAlive { get; set; }
    }
}
