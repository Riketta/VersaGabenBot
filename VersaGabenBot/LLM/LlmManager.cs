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

        public async Task<string> ProcessMessageAsync(SocketUserMessage message, Guild guild, ChannelRepository channelRepository, bool botMentioned)
        {
            if (string.IsNullOrEmpty(message.CleanContent)) // TODO: process attachments.
                return null;

            bool botRandomReply = new Random().NextDouble() <= guild.LlmOptions.RandomReplyChance;
            if (!botMentioned && !botRandomReply)
                return null;

            using IDisposable typing = message.Channel.EnterTypingState();

            string formatted;
            if (_options.IncludeMessageSender)
                formatted = _options.MessageWithSenderTemplate
                    .Replace(_options.SenderPlaceholder, message.Author.GlobalName ?? message.Author.Username)
                    .Replace(_options.MessagePlaceholder, message.CleanContent);
            else
                formatted = message.Content;
            LlmMessage llmMessage = new LlmMessage(Roles.User, formatted);

            // TODO: replace ChannelRepository with smth like IHistoryReader?
            var messages = await channelRepository.GetMessages(message.Channel.Id, guild.LlmOptions.MessagesContextSize);
            var llmMessages = messages.Select(m => new LlmMessage(m)); // TODO: probably extremely inefficient.
            LlmMessage response = await _client.GenerateTextAsync(llmMessages);

            if (_options.RemoveEmptyLines)
                response.RemoveConsecutiveEmptyLines(_options.MaxEmptyLines);

            return response.Content;
        }
    }
}
