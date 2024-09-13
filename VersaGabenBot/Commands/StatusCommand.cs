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
            ulong channelId = command.ChannelId.Value;
            if (!await _guildRepository.IsGuildRegistered(channelId))
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.Error("Guild not registered!").Build(), ephemeral: true);
                return;
            }

            if (!await _channelRepository.IsChannelRegistered(channelId))
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.Error("Channel not registered!").Build(), ephemeral: true);
                return;

            var messagesCount = await _channelRepository.GetMessagesCount(channelId);
            var maxLlmMessagesCount = (await _guildRepository.GetGuildLlmOptions(channelId)).MessagesContextSize;

            string[] reports =
            [
                $"Current history length: {messagesCount}.",
                $"LLM history length: {Math.Min(messagesCount, maxLlmMessagesCount)}.",
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
