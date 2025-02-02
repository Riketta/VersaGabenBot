using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot.LLM
{
    internal class LlmManager
    {
        private readonly ModelOptions _modelOptions;
        private readonly ILlmClient _client;

        public LlmManager(ModelOptions modelOptions, ILlmClient client)
        {
            _modelOptions = modelOptions;
            _client = client;
        }

        public async Task<string> ProcessMessageAsync(ulong selfId, IEnumerable<Message> messages, ChannelLlmOptions llmOptions)
        {
            Model model = _modelOptions.Models.FirstOrDefault(m => m.Name == llmOptions.Model) ?? _modelOptions.Models.First();
            if (model is null)
                return null;

            // TODO: process attachments.

            // TODO: probably extremely inefficient.
            var llmMessages = messages.Select(m =>
            {
                if (model.IncludeMessageSender && m.UserID != selfId)
                    return new LlmMessage(m, model.MessageWithSenderTemplate, model.SenderPlaceholder, model.MessagePlaceholder);
                else
                    return new LlmMessage(m);
            });
            LlmMessage response = await _client.GenerateTextAsync(model, llmMessages, llmOptions.SystemPrompt ?? model.DefaultSystemPrompt);

            if (model.RemoveEmptyLines)
                response.RemoveConsecutiveEmptyLines(model.MaxEmptyLines);

            return response.Content;
        }
    }
}
