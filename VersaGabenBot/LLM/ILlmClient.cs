using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.LLM
{
    internal interface ILlmClient
    {
        public Task<LlmMessage> GenerateTextAsync(string message);
        public Task<LlmMessage> GenerateTextAsync(LlmMessage message);
        public Task<LlmMessage> GenerateTextAsync(IEnumerable<LlmMessage> messages);
    }
}
