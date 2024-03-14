using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Timer = System.Timers.Timer;

namespace VersaGabenBot
{
    public class Bot
    {
        private static Logger logger = Logger.GetLogger();

        private static readonly Random random = new Random();
        private static readonly string tokenFile = "token.txt";

        private static readonly string[] statusList = new string[] { "Warzone: Zombie Piglin", "The Last of Us: 3060", "Warcraft Fishman", "Hahullbreaker", "Ghost of Kijuw", "Metro 2077: Rusted", "Dota 3", "CS: GO HOME", "World of Minecraft", "Doomstiny", "Fortignite", "WTS Boost", "Battle for LootBox", "Overbotch 2", "Photone Rush", "Cyberpunk 1337", "StarCraft Dungeons" };
        private static readonly string[] statusList = new string[] { "Zap Volt Overcharge", "Hahool: Kazovstan", "Warzone: Zombie Piglin", "The Last of Us: 3060", "Warcraft Fishman", "Hahullbreaker", "Ghost of Kijuw", "Metro 2077: Rusted", "Dota 3", "CS: GO HOME", "World of Minecraft", "Doomstiny", "Fortignite", "WTS Boost", "Battle for LootBox", "Overbotch 2", "Photone Rush", "Cyberpunk 1337", "StarCraft Dungeons" };
        private static readonly int statusTimeout = 60 * 45 * 1000;
        private static readonly Timer statusTimer = new Timer();

        private static readonly ulong gabenGuildId = 293649968865214464;
        private static readonly ulong botzoneChannelId = 649913439556206592;
        private static readonly ulong generalChannelId = 293649968865214464;

        //private static readonly IServiceProvider _services = ConfigureServices();

        private static readonly DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            // How much logging do you want to see?
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50,
            GatewayIntents = GatewayIntents.All,
            //WebSocketProvider = WS4NetProvider.Instance
        });

        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private static readonly CommandService _commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
        });

        public static async Task Start()
        {
            var token = File.ReadAllText(tokenFile);

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
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            statusTimer.Interval = statusTimeout;
            statusTimer.Elapsed += StatusTimer_Elapsed;
            statusTimer.Start();
            logger.Info("Initial status: \"{0}\"", SetRandomStatus());
        }

        private static async Task Client_UserIsTyping(Cacheable<IUser, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {
            if (arg1.Value.Username == "Riketta")
                await (_client.GetChannel(botzoneChannelId) as ISocketMessageChannel).SendMessageAsync("DEBUG");
        }

        private static async Task Client_InviteDeleted(SocketGuildChannel channel, string code)
        {
            string message = string.Format("[InviteDeleted] \"{0}\" for \"{1}\"", code, channel?.Name ?? "-");
            logger.Info(message);
            await (_client.GetChannel(botzoneChannelId) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private static async Task Client_InviteCreated(SocketInvite invite)
        {
            string message = string.Format("[InviteCreated] \"{0}\" (\"{1}\"): \"{2}\" for \"{3}\"", invite.Inviter.Username, invite.Inviter.Nickname ?? "-", invite.Code, invite.TargetUser?.Username ?? "-");
            logger.Info(message);
            await(_client.GetChannel(botzoneChannelId) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private static async Task Client_UserLeft(SocketGuild guild, SocketUser user)
        {
            string message = string.Format("[UserLeft] \"{0}\"", user.Username);
            logger.Info(message);
            await (_client.GetChannel(botzoneChannelId) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private static async Task Client_UserJoined(SocketGuildUser user)
        {
            string message = string.Format("[UserJoined] \"{0}\"", user.Username);
            logger.Info(message);
            await (_client.GetChannel(botzoneChannelId) as ISocketMessageChannel).SendMessageAsync(message);
        }

        private static void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.Info("Updating status: \"{0}\"", SetRandomStatus());
        }

        private static string SetRandomStatus()
        {
            string status = statusList[random.Next(statusList.Length)];
            _client.SetActivityAsync(new Game(status));

            return status;
        }

        private static async Task Client_MessageReceived(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            if (message.Author.Id == _client.CurrentUser.Id || message.Author.IsBot) return;
            if (message.Channel.Id != generalChannelId && message.Channel.Id != botzoneChannelId) return;

            if (message.Content.Contains("🇺🇦"))
                await message.AddReactionAsync(new Emoji("🐷"));
            else if (message.Content.Contains("🐸"))
                await message.AddReactionAsync(new Emoji("🐸"));
            else if (message.Content.Contains("🇷🇺"))
                await message.AddReactionAsync(new Emoji("🐻‍❄️"));
            else if (message.Content.ToLower().Contains("хахoл"))
            {
                    await message.AddReactionAsync(new Emoji("🙋🏿‍♂️"));
                    await message.AddReactionAsync(new Emoji("🙋🏿"));
            }
            else
            {
                foreach (var mention in message.MentionedUsers)
                    if (mention.Id == _client.CurrentUser.Id)
                    {
                        await message.Channel.SendMessageAsync("🐸");
                        logger.Debug(message.Content);
                        break;
                    }
            }
        }

        private static Task Log(LogMessage message)
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
        //private static IServiceProvider ConfigureServices()
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

        //private static async Task InitCommands()
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

        //private static async Task HandleCommandAsync(SocketMessage arg)
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