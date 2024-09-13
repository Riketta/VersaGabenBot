using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot.LLM
{
    internal class LlmMessage
    {
        [JsonPropertyName("role")]
        public Roles Role { get; private set; }

        [JsonPropertyName("content")]
        public string Content { get; private set; }

        [JsonPropertyName("images")]
        public List<byte[]> Images { get; private set; } // TODO: handle images.

        [JsonConstructor]
        public LlmMessage(Roles role, string content, List<byte[]> images = null)
        {
            Role = role;
            Content = content;
            Images = images;
        }

        public LlmMessage(Message message)
        {
            Role = message.LlmRole;
            Content = message.Content;
        }

        public LlmMessage(Message message, string template, string senderPlaceholder, string messagePlaceholder)
        {
            Role = message.LlmRole;
            Content = template
                .Replace(senderPlaceholder, message.Username)
                .Replace(messagePlaceholder, message.Content);
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
