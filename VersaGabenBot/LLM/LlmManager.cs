using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;
using VersaGabenBot.Options;

namespace VersaGabenBot.LLM
{
    internal class LlmManager
    {
        private readonly LlmOptions _options;
        private readonly ILlmClient _client;

        public LlmManager(LlmOptions options, ILlmClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task<string> ProcessMessageAsync(ulong selfId, IEnumerable<Message> messages)
        {
            // TODO: process attachments.

            // TODO: probably extremely inefficient.
            var llmMessages = messages.Select(m =>
            {
                if (_options.IncludeMessageSender && m.UserID != selfId)
                    return new LlmMessage(m, _options.MessageWithSenderTemplate, _options.SenderPlaceholder, _options.MessagePlaceholder);
                else
                    return new LlmMessage(m);
            });
            LlmMessage response = await _client.GenerateTextAsync(llmMessages);

            if (_options.RemoveEmptyLines)
                response.RemoveConsecutiveEmptyLines(_options.MaxEmptyLines);

            return response.Content;
        }
    }
}
