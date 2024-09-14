using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Repositories;

namespace VersaGabenBot.Commands
{
    internal class UnregisterCommand : IGlobalCommand
    {
        private readonly GuildRepository _guildRepository;
        private readonly ChannelRepository _channelRepository;
        private readonly ulong _adminID; // TODO: temp hack, replace with authentication system.

        public string Name => "unregister";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public UnregisterCommand(GuildRepository guildRepository, ChannelRepository channelRepository, ulong adminId)
        {
            _guildRepository = guildRepository;
            _channelRepository = channelRepository;
            _adminID = adminId;
        }

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Unregister the current channel and guild if there are no registered channels left.")
                .Build();

            return command;
        }

        public async Task Handle(SocketSlashCommand command)
        {
            if (command.User.Id != _adminID)
            {
                await command.RespondAsync(embed: TemplateEmbedBuilder.ErrorUnauthorizedCommandUse().Build(), ephemeral: true);
                return;
            }

            ulong guildId = command.GuildId.Value;
            if (!await _guildRepository.IsGuildRegistered(guildId))
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

            await _channelRepository.UnregisterChannel(channelId);

            // TODO: handle guild unregistration?

            var embed = new EmbedBuilder()
                .WithTitle("Channel unregistration")
                .WithDescription($"Channel ({channelId}) successfully unregistered.")
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed, ephemeral: true);
        }
    }
}
