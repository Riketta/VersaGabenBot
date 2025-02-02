using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.LLM
{
    internal interface ILlmClient
    {
        public Task<LlmMessage> GenerateTextAsync(Model model, string message);
        public Task<LlmMessage> GenerateTextAsync(Model model, LlmMessage message);
        public Task<LlmMessage> GenerateTextAsync(Model model, IEnumerable<LlmMessage> messages);
        public Task<LlmMessage> GenerateTextAsync(Model model, IEnumerable<LlmMessage> messages, string systemPrompt);
    }
}
