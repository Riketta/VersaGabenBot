using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VersaGabenBot.Ollama
{
    internal class Message
    {
        [JsonPropertyName("role")]
        public Roles Role { get; private set; }

        [JsonPropertyName("content")]
        public string Content { get; private set; }

        [JsonPropertyName("images")]
        public List<byte[]> Images { get; private set; }

        public Message(Roles role, string content, List<byte[]> images = null)
        {
            Role = role;
            Content = content;
            Images = images;
        }

        public override string ToString()
        {
            return $"{Role.ToString().ToUpper()} \"\"\"{Content}\"\"\"";
        }
    }
}
