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

        public string Name => "status";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public StatusCommand(GuildRepository guildRepository)
        {
            _guildRepository = guildRepository;
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
            Guild guild = _guildRepository.GetGuildByChannelUUID(channelId);
            if (guild is null)
                return;

            ConcurrentQueue<Message> channelHistory = null;
            if (guild.MessageHistoryPerChannel.TryGetValue(channelId, out ConcurrentQueue<Message> value))
                channelHistory = value;

            string[] reports =
            [
                $"Current history length: {(channelHistory is null ? 0 : channelHistory.Count)}.",
                $"LLM history length: {(channelHistory is null ? 0 : channelHistory.TakeLast(guild.Options.LlmOptions.MessagesContextSize)?.Count() ?? 0)}.",
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
