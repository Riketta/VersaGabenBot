using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.LLM
{
    internal interface ILlmClient
    {
        public Task<Message> GenerateTextAsync(string message);
        public Task<Message> GenerateTextAsync(Message message);
        public Task<Message> GenerateTextAsync(IEnumerable<Message> messages);
    }
}
