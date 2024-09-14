using Discord.Rest;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.LLM;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;
using System.Reactive;

namespace VersaGabenBot.Commands
{
    internal class PushMessageCommand : IGlobalCommand
    {
        private const string OptionNameRole = "role";
        private const string OptionNameUser = "user";
        private const string OptionNameMessage = "message";
        private readonly GuildRepository _guildRepository;
        private readonly ChannelRepository _channelRepository;

        public string Name => "push";
        public RestGlobalCommand RestGlobalCommand { get; set; }
        public RestApplicationCommand RestApplicationCommand => RestGlobalCommand;

        public PushMessageCommand(GuildRepository guildRepository, ChannelRepository channelRepository)
        {
            _guildRepository = guildRepository;
            _channelRepository = channelRepository;
        }

        public SlashCommandProperties Prepare()
        {
            var command = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Push message to channel history.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(OptionNameRole)
                    .WithDescription("The LLM role")
                    .AddChoice(Roles.System.ToString(), (int)Roles.System) // TODO: generate from existing enum?
                    .AddChoice(Roles.User.ToString(), (int)Roles.User)
                    .AddChoice(Roles.Assistant.ToString(), (int)Roles.Assistant)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithRequired(true)
                )
                // TODO: for non-authorized users only let self and bot.
                .AddOption(OptionNameUser, ApplicationCommandOptionType.User, "The user to fake message from.", isRequired: true)
                .AddOption(OptionNameMessage, ApplicationCommandOptionType.String, "The message content.", isRequired: true)
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

            Roles llmRole = (Roles)(long)command.Data.Options.Single(o => o.Name == OptionNameRole).Value;
            SocketUser user = (SocketUser)command.Data.Options.Single(o => o.Name == OptionNameUser).Value;
            string content = (string)command.Data.Options.Single(o => o.Name == OptionNameMessage).Value;

            Message message = new Message()
            {
                MessageID = 0, // TODO: dangerous?
                ChannelID = channelId,
                UserID = user.Id,
                Username = user.GetGlobalNameOrUsername(),
                Timestamp = DateTime.Now, // TODO: UTC?
                LlmRole = llmRole,
                BotRelated = true,
                Content = content,
            };
            await _channelRepository.InsertMessage(message);

            string report =
                $"Message from user \"{message.Username}\" with LLM role **{llmRole}** pushed to history with following content:{Environment.NewLine}```{message.Content}```";

            var embed = new EmbedBuilder()
                .WithTitle("Message pushed to channel history")
                .WithDescription(report)
                .WithColor(Color.Green)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed);
        }
    }
}
