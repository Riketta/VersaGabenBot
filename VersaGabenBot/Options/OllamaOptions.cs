using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Ollama;

namespace VersaGabenBot.Options
{
    internal class OllamaOptions : IOptions
    {
        public string OllamaApiUrl { get; set; } = "http://localhost:11434/";
        public string Model { get; set; } = "phi3.5";
        public bool Multimodal { get; set; } = false;
        public string MaxTokens { get; set; } = "http://localhost:11434/";
        public bool Stream { get; set; } = false;
        public int Temperature { get; set; } = 0;
        public List<Message> SetupMessages { get; set; } =
        [
            new Message("You are a funny public chat bot."),
        ];
    }
}
