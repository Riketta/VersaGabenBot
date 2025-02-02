#nullable enable

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using VersaGabenBot.Ollama;
using VersaGabenBot.LLM;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Repositories;

namespace VersaGabenBot
{
    class Program
    {
        private static Logger logger = Logger.GetLogger();

        public static Task Main(string[] args) => new Program().MainAsync();
        public async Task MainAsync()
        {
            var config = Config.LoadOrCreateDefault(Config.DefaultConfigPath);

            var dbContext = new DatabaseContext(config.DatabaseConfig);
            DatabaseInitializer dbInitializer = new DatabaseInitializer(dbContext);
            dbInitializer.Initialize();
            GuildRepository guildRepository = new GuildRepository(dbContext);
            ChannelRepository channelRepository = new ChannelRepository(dbContext);

            var assemblyName = Assembly.GetEntryAssembly().GetName();
            Console.Title = string.Format("{0} ver. {1}", assemblyName.Name, assemblyName.Version);
            logger.Info(Console.Title);

            OllamaClient ollamaClient = new OllamaClient(config.OllamaOptions);
            LlmManager llmManager = new LlmManager(config.ModelOptions, ollamaClient);

            Bot bot = new Bot(config.BotConfig, config.ProxyOptions, dbContext, llmManager, guildRepository, channelRepository);
            await bot.Start();

            await Task.Delay(Timeout.Infinite);
        }
    }
}
