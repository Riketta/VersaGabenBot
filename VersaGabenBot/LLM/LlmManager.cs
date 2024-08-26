using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Guilds;
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

        public async Task ProcessMessageAsync(SocketSelfUser currentUser, SocketUserMessage message, Guild guild)
        {
            // Store messages in history that not even addressed to bot.
            if (!_options.OnlySaveChatHistoryRelatedToBot)
                guild.AppendMessage(message.Channel.Id, new Message(Roles.User, message));

            bool isRandomReply = new Random().NextDouble() <= _options.RandomReplyChance;
            bool isMentioned = message.MentionedUsers.Any(user => user.Id == currentUser.Id);

            if (!isMentioned && !isRandomReply)
                return;

            IDisposable typing = message.Channel.EnterTypingState();

            if (_options.OnlySaveChatHistoryRelatedToBot)
                guild.AppendMessage(message.Channel.Id, new Message(Roles.User, message));
            Message response = await _client.GenerateTextAsync(guild.MessageHistoryPerChannel[message.Channel.Id].ToArray());
            guild.AppendMessage(message.Channel.Id, response);

            await message.ReplyAsync(response.Content);
        }
    }
}
