using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;

namespace VersaGabenBot.LLM
{
    internal class LlmMessage
    {
        [JsonPropertyName("role")]
        public Roles Role { get; private set; }

        [JsonPropertyName("content")]
        public string Content { get; private set; }

        [JsonPropertyName("images")]
        public List<byte[]> Images { get; private set; }

        [JsonConstructor]
        public LlmMessage(Roles role, string content, List<byte[]> images = null)
        {
            Role = role;
            Content = content;
            Images = images;
        }

        public LlmMessage(Message message)
        {
            Role = message.AuthorRole;
            Content = message.Content;
            Images = null; // TODO: handle images.
        }

        public void RemoveConsecutiveEmptyLines(int maxEmptyLines = 1)
        {
            Content = Content.RemoveConsecutiveEmptyLines(maxEmptyLines);
        }

        public override string ToString()
        {
            return $"{Role.ToString().ToUpper()} \"\"\"{Content}\"\"\"";
        }
    }
}
