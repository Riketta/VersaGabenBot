﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

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

            Bot bot = new Bot(config.BotConfig);
            await bot.Start();

            await Task.Delay(Timeout.Infinite);
        }
    }
}
