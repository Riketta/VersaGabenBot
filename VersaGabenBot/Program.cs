using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using VersaGabenBot.Ollama;

namespace VersaGabenBot
{
    class Program
    {
        private static Logger logger = Logger.GetLogger();

        public static Task Main(string[] args) => new Program().MainAsync();
        public async Task MainAsync()
        {
            var config = Config.LoadOrCreateDefault(Config.DefaultConfigPath);

            var assemblyName = Assembly.GetEntryAssembly().GetName();
            Console.Title = string.Format("{0} ver. {1}", assemblyName.Name, assemblyName.Version);
            logger.Info(Console.Title);

            OllamaClient ollamaClient = new OllamaClient(config.OllamaOptions);
            Bot bot = new Bot(config.BotConfig, ollamaClient);
            GuildManager guildManager = new GuildManager(config.GuildOptions);

            // Sample guild registration.
            //Guild guild = guildManager.RegisterGuild(293649968865214464);
            //guild.BotChannelID = 649913439556206592;
            //guild.RegisterChannel(guild.BotChannelID);
            //guild.RegisterChannel(293649968865214464);
            //config.Save();

            await bot.Start();

            await Task.Delay(Timeout.Infinite);
        }
    }
}
