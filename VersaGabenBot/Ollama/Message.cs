using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Ollama
{
    internal class Message
    {
        public Roles Role { get; private set; }
        public string Content { get; private set; }
        public List<byte[]> Images { get; private set; }

        public Message(Roles role, string message, List<byte[]> images = null)
        {
            Role = role;
            Content = message;
            Images = images;
        }

        public override string ToString()
        {
            return $"{Role.ToString().ToUpper()} \"\"\"{Content}\"\"\"";
        }
    }
}
