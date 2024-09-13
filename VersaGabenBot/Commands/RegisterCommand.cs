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
    /// <summary>
    /// Register channel (and guild, if necessary) by channel ID.
    /// </summary>
    internal class RegisterCommand : IGlobalCommand
    {
        private readonly GuildRepository _guildRepository;
        private readonly ChannelRepository _channelRepository;
        private readonly ulong _adminID; // TODO: temp hack, replace with authentication system.

        public string Name => "register";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public RegisterCommand(GuildRepository guildRepository, ChannelRepository channelRepository, ulong adminId)
        {
            _guildRepository = guildRepository;
            _channelRepository = channelRepository;
            _adminID = adminId;
        }

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Register current channel and guild if it is not yet registered.")
                .Build();

            return command;
        }

        public async Task Handle(SocketSlashCommand command) // TODO: validate authorization.
        {
            if (command.User.Id != _adminID)
                return;

            ulong guildId = command.GuildId.Value;
            if (!await _guildRepository.IsGuildRegistered(guildId))
                await _guildRepository.RegisterGuild(guildId);

            ulong channelId = command.ChannelId.Value;
            if (!await _channelRepository.IsChannelRegistered(channelId))
                await _channelRepository.RegisterChannel(channelId, guildId);

            var embed = new EmbedBuilder()
                .WithTitle("Guild & Channel registration")
                .WithDescription($"Guild ({guildId}) and channel ({channelId}) successfully registered.")
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed, ephemeral: true);
        }
    }
}
