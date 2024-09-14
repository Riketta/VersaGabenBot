using Discord.Rest;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;

namespace VersaGabenBot.Commands
{
    internal class ResetCutoffCommand : IGlobalCommand
    {
        private readonly ChannelRepository _channelRepository;

        public string Name => "clear";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public ResetCutoffCommand(ChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Reset messages cutoff for LLM context for current channel.")
                .Build();

            return command;
        }

        public async Task Handle(SocketSlashCommand command)
        {
            ulong channelId = command.ChannelId.Value;
            if (!await _channelRepository.IsChannelRegistered(channelId))
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.ErrorChannelNotRegistered().Build(), ephemeral: true);
                return;
            }

            DateTime newCutoff = command.CreatedAt.UtcDateTime;
            await _channelRepository.UpdateChannelCutoff(channelId, command.CreatedAt.UtcDateTime);

            var embed = new EmbedBuilder()
                .WithTitle("New history cutoff")
                .WithDescription($"{newCutoff}")
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed);
        }
    }
}
