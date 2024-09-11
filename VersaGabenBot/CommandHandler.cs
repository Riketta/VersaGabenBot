using Discord.Net;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Commands;
using Discord.Rest;

namespace VersaGabenBot
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly Dictionary<string, ICommand> _commands;

        public CommandHandler(DiscordSocketClient client, List<ICommand> commands)
        {
            _client = client;
            _commands = commands.ToDictionary(c => c.Name);
        }

        public async Task RegisterCommands()
        {
            foreach (var command in _commands.Values)
            {
                try
                {
                    var commandProps = command.Prepare();
                    if (command is IGlobalCommand globalCommand)
                    {
                        RestGlobalCommand restGlobalCommand = await _client.Rest.CreateGlobalCommand(commandProps);
                        globalCommand.RestGlobalCommand = restGlobalCommand; // TODO: fix excessive property accessibility.
                    }
                    else if (command is IGuildCommand guildCommand)
                        throw new NotImplementedException();
                }
                catch (HttpException exception)
                {
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public async Task HandleCommands(SocketSlashCommand slashCommand)
        {
            var command = _commands[slashCommand.Data.Name];
            if (command is null)
                return; // TODO: warn that somehow registered command can't be processed?

            await command.Handle(slashCommand);
        }
    }
}
