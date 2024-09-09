using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
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

        public async Task<string> ProcessMessageAsync(SocketSelfUser currentUser, SocketUserMessage message, Guild guild)
        {
            if (string.IsNullOrEmpty(message.CleanContent)) // TODO: process attachments.
                return null;

            string formatted;
            if (_options.IncludeMessageSender)
                formatted = _options.MessageWithSenderTemplate
                    .Replace(_options.SenderPlaceholder, message.Author.GlobalName ?? message.Author.Username)
                    .Replace(_options.MessagePlaceholder, message.CleanContent);
            else
                formatted = message.Content;
            Message llmMessage = new Message(Roles.User, formatted);

            // Store messages in history that not even addressed to bot.
            if (!guild.Options.LlmOptions.OnlySaveChatHistoryRelatedToBot)
                guild.AppendMessage(message.Channel.Id, llmMessage);

            bool isRandomReply = new Random().NextDouble() <= guild.Options.LlmOptions.RandomReplyChance;
            bool isMentioned = message.MentionedUsers.Any(user => user.Id == currentUser.Id);

            if (!isMentioned && !isRandomReply)
                return null;

            using IDisposable typing = message.Channel.EnterTypingState();

            if (guild.Options.LlmOptions.OnlySaveChatHistoryRelatedToBot)
                guild.AppendMessage(message.Channel.Id, llmMessage);
            Message response = await _client.GenerateTextAsync(guild.MessageHistoryPerChannel[message.Channel.Id].Take(guild.Options.LlmOptions.MessagesContextSize).ToArray());
            if (_options.RemoveEmptyLines)
                response.RemoveConsecutiveEmptyLines(_options.MaxEmptyLines);
            guild.AppendMessage(message.Channel.Id, response);

            return response.Content;
        }
    }
}
