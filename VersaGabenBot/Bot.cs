using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using VersaGabenBot.Commands;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Data.Repositories;
using VersaGabenBot.LLM;
using VersaGabenBot.Ollama;
using VersaGabenBot.Options;
using Timer = System.Timers.Timer;

namespace VersaGabenBot
{
    internal class Bot
    {
        private Logger logger = Logger.GetLogger(); // TODO: inject.

        private readonly Random random = new Random();
        private readonly Timer statusTimer = new Timer();

        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;

        private readonly GuildRepository _guildRepository;
        private readonly ChannelRepository _channelRepository;

        private readonly BotConfig _config;
        private readonly DatabaseContext _db;
        private readonly LlmManager _llmManager;

        //private readonly IServiceProvider _services = ConfigureServices();

        public Bot(BotConfig config, DatabaseContext db, LlmManager llmManager, GuildRepository guildRepository, ChannelRepository channelRepository)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50,
                GatewayIntents = GatewayIntents.All,
            });
            List<ICommand> commands = new List<ICommand>()
            {
                new StatusCommand(guildRepository, channelRepository),
                new RegisterCommand(guildRepository, channelRepository),
                new UnregisterCommand(guildRepository, channelRepository),
                new SaveCommand(),
                new WipeCommand(),
                new ClearCommand(),
            };
            _commandHandler = new CommandHandler(_client, commands);

            _config = config;
            _db = db;
            _llmManager = llmManager;

            _guildRepository = guildRepository;
            _channelRepository = channelRepository;
        }

        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private readonly CommandService _commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
        });

        public async Task Start()
        {
            _client.Ready += _commandHandler.RegisterCommands;
            _client.SlashCommandExecuted += _commandHandler.HandleCommands;

            _client.Log += Log;
            _client.MessageReceived += Client_MessageReceived;
            _client.UserJoined += Client_UserJoined;
            _client.UserLeft += Client_UserLeft;
            _client.InviteCreated += Client_InviteCreated;
            _client.InviteDeleted += Client_InviteDeleted;
            //_client.UserIsTyping += Client_UserIsTyping; // DEBUG

            _commands.Log += Log;
            // Centralize the logic for commands into a separate method.
            //await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();

            statusTimer.Interval = _config.StatusUpdateInterval;
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Start();
            logger.Info("Initial status: \"{0}\"", SetRandomStatus());
        }

        private async Task Client_InviteDeleted(SocketGuildChannel channel, string code)
        {
            Guild guild = await _guildRepository.GetGuildByChannelID(channel.Id);
            if (guild is null) return;

            string message = string.Format("[InviteDeleted] \"{0}\" for \"{1}\"", code, channel?.Name ?? "-");
            logger.Info(message);
            if (guild.SystemChannelID is not null)
                await (_client.GetChannel(guild.SystemChannelID.Value) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private async Task Client_InviteCreated(SocketInvite invite)
        {
            Guild guild = await _guildRepository.GetGuildByChannelID(invite.Guild.Id);
            if (guild is null) return;

            string message = string.Format("[InviteCreated] \"{0}\" (\"{1}\"): \"{2}\" for \"{3}\"", invite.Inviter.Username, invite.Inviter.Nickname ?? "-", invite.Code, invite.TargetUser?.Username ?? "-");
            logger.Info(message);
            if (guild.SystemChannelID is not null)
                await (_client.GetChannel(guild.SystemChannelID.Value) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private async Task Client_UserLeft(SocketGuild socketGuild, SocketUser user)
        {
            Guild guild = await _guildRepository.GetGuildByChannelID(socketGuild.Id);
            if (guild is null) return;

            string message = string.Format("[UserLeft] \"{0}\"", user.Username);
            logger.Info(message);
            if (guild.SystemChannelID is not null)
                await (_client.GetChannel(guild.SystemChannelID.Value) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            Guild guild = await _guildRepository.GetGuildByChannelID(user.Guild.Id);
            if (guild is null) return;

            string message = string.Format("[UserJoined] \"{0}\"", user.Username);
            logger.Info(message);
            if (guild.SystemChannelID is not null)
                await (_client.GetChannel(guild.SystemChannelID.Value) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.Info("Updating status: \"{0}\"", SetRandomStatus());
        }

        private string SetRandomStatus()
        {
            string status = _config.StatusList[random.Next(_config.StatusList.Count)];
            _client.SetActivityAsync(new Game(status));

            return status;
        }

        private async Task Client_MessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage is not SocketUserMessage userMessage) return;
            if (userMessage.Author.Id == _client.CurrentUser.Id || userMessage.Author.IsBot) return;

            // If the guild is not found, the message channel is not registered and should not be processed further.
            // An exception is a request to register a channel from a person with such rights.
            Guild guild = await _guildRepository.GetGuildByChannelID(userMessage.Channel.Id);
            if (guild is null) return;

            bool botMentioned = userMessage.MentionedUsers.Any(user => user.Id == _client.CurrentUser.Id);
            Message message = new Message(userMessage, Roles.User, botMentioned);
            await _channelRepository.InsertMessage(message);

            // TODO: implement as a slash commands.
            if (userMessage.Content.StartsWith("!wipe"))
            {
                await _channelRepository.DeleteAllMessages(userMessage.Channel.Id);
                return;
            }
            else if (userMessage.Content.StartsWith("!save"))
            {
                await _db.Save();
                return;
            }

            async Task blockingLlmTask() // This task should not be awaited.
            {
                string response = await _llmManager.ProcessMessageAsync(userMessage, guild, _channelRepository, botMentioned);
                string[] messages = response?.SplitByLengthAtNewLine(_config.MaxMessageLength);

                if (messages is null || messages.Length == 0)
                    return;

                foreach (var llmResponseContent in messages)
                {
                    var llmResponse = await userMessage.ReplyAsync(llmResponseContent);
                    var llmMessage = new Message(llmResponse, Roles.Assistant, true);
                    await _channelRepository.InsertMessage(message); // TODO: store messages and insert as a batch at once.
                }
            }
            _ = Task.Run(blockingLlmTask);
        }

        private Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            logger.Info($"[{message.Severity}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            // return Task.Delay(0); // for .NET 4.5.2 or lower
            return Task.CompletedTask;
        }

        // If any services require the client, or the CommandService, or something else you keep on hand,
        // pass them as parameters into this method as needed.
        // If this method is getting pretty long, you can seperate it out into another file using partials.
        //private IServiceProvider ConfigureServices()
        //{
        //    var map = new ServiceCollection()
        //        // Repeat this for all the service classes
        //        // and other dependencies that your commands might need.
        //        .AddSingleton(new SomeServiceClass());

        //    // When all your required services are in the collection, build the container.
        //    // Tip: There's an overload taking in a 'validateScopes' bool to make sure
        //    // you haven't made any mistakes in your dependency graph.
        //    return map.BuildServiceProvider();
        //}

        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.

        //private async Task InitCommands()
        //{
        //    // Either search the program and add all Module classes that can be found.
        //    // Module classes MUST be marked 'public' or they will be ignored.
        //    // You also need to pass your 'IServiceProvider' instance now,
        //    // so make sure that's done before you get here.
        //    await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        //    // Or add Modules manually if you prefer to be a little more explicit:
        //    //await _commands.AddModuleAsync<SomeModule>(_services);
        //    // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

        //    // Subscribe a handler to see if a message invokes a command.
        //    _client.MessageReceived += HandleCommandAsync;
        //}

        //private async Task HandleCommandAsync(SocketMessage arg)
        //{
        //    // Bail out if it's a System Message.
        //    var message = arg as SocketUserMessage;
        //    if (message == null) return;

        //    if (message.Author.Id == _client.CurrentUser.Id || message.Author.IsBot) return;

        //    int pos = 0;
        //    if (message.HasMentionPrefix(_client.CurrentUser, ref pos)) // msg.HasCharPrefix('!', ref pos)
        //    {
        //        var context = new SocketCommandContext(_client, message);

        //        var result = await _commands.ExecuteAsync(context, pos, _services);

        //        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
        //            await message.Channel.SendMessageAsync(result.ErrorReason);
        //    }
        //}
    }
}