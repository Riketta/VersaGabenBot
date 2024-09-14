using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Commands
{
    internal class StatusCommand : IGlobalCommand
    {
        private readonly GuildRepository _guildRepository;
        private readonly ChannelRepository _channelRepository;

        public string Name => "status";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public StatusCommand(GuildRepository guildRepository, ChannelRepository channelRepository)
        {
            _guildRepository = guildRepository;
            _channelRepository = channelRepository;
        }

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Print current status and extra info specific for the current channel.")
                .Build();

            return command;
        }

        public async Task Handle(SocketSlashCommand command)
        {
            ulong guildId = command.GuildId.Value;
            Guild guild = await _guildRepository.GetGuild(guildId);
            if (guild is null)
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.ErrorGuildNotRegistered().Build(), ephemeral: true);
                return;
            }

            ulong channelId = command.ChannelId.Value;
            if (!await _channelRepository.IsChannelRegistered(channelId))
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.ErrorChannelNotRegistered().Build(), ephemeral: true);
                return;
            }

            uint currentMessagesCount = await _channelRepository.GetMessagesCount(channelId);
            uint maxMessagesCount = guild.Options.MessageHistoryLimitPerChannel;
            uint currentLlmMessagesCount = Math.Min(currentMessagesCount, guild.LlmOptions.MessagesContextSize);
            uint maxLlmMessagesCount = guild.LlmOptions.MessagesContextSize;

            List<Message> messages = await _channelRepository.GetMessagesWithCutoff(channelId, guild.LlmOptions.MessagesContextSize);
            Message firstMessage = messages.First();
            Message lastMessage = messages.Last();
            string firstMessageReference = "MISSING";
            string lastMessageReference = firstMessageReference;
            try
            {
                firstMessageReference = (await command.Channel.GetMessageAsync(firstMessage.MessageID)).GetJumpUrl();
            }
            catch { }
            try
            {
                lastMessageReference = (await command.Channel.GetMessageAsync(lastMessage.MessageID)).GetJumpUrl();
            }
            catch { }

            string[] reports =
            [
                $"Current history length: {currentMessagesCount}/{maxMessagesCount}.",
                $"LLM history length: {currentLlmMessagesCount}/{maxLlmMessagesCount}.",
                Environment.NewLine,
                $"First LLM message ({firstMessage.Timestamp} - {firstMessageReference}): ```{firstMessage.Content}```",
                $"Last LLM message ({lastMessage.Timestamp} - {lastMessageReference}): ```{lastMessage.Content}```",
            ];

            var embed = new EmbedBuilder()
                .WithTitle("Status")
                .WithDescription(string.Join(Environment.NewLine, reports))
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed, ephemeral: true);
        }
    }
}
