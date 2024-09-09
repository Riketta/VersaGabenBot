using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VersaGabenBot.LLM
{
    internal class Message
    {
        [JsonPropertyName("role")]
        public Roles Role { get; private set; }

        [JsonPropertyName("content")]
        public string Content { get; private set; }

        [JsonPropertyName("images")]
        public List<byte[]> Images { get; private set; }

        [JsonConstructor]
        public Message(Roles role, string content, List<byte[]> images = null)
        {
            Role = role;
            Content = content;
            Images = images;
        }

        public Message(Roles role, SocketUserMessage message)
        {
            Role = role;
            Content = message.Content;
            Images = null; // TODO: process attachments.
        }

        public void RemoveConsecutiveEmptyLines(int maxEmptyLines = 1)
        {
            StringBuilder sb = new StringBuilder(Content.Length);
            int emptyLineCount = 0;

            foreach (string line in Content.Split('\n'))
                if (string.IsNullOrWhiteSpace(line))
                {
                    emptyLineCount++;
                    if (emptyLineCount <= maxEmptyLines)
                        sb.Append('\n');
                }
                else
                {
                    emptyLineCount = 0;
                    sb.Append(line.TrimEnd()).Append('\n'); // TODO: is this TrimEnd required?
                }

            Content = sb.ToString().TrimEnd();
        }

        public override string ToString()
        {
            return $"{Role.ToString().ToUpper()} \"\"\"{Content}\"\"\"";
        }
    }
}
