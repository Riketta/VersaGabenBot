using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot
{
    internal class Config
    {
        public static readonly string DefaultConfigPath = $"{nameof(VersaGabenBot)}.json";

        /// <summary>
        /// Path associated with the current config instance.
        /// </summary>
        [JsonIgnore]
        public string PathToConfig { get; private set; }

        #region Config Fields
        public BotConfig BotConfig { get; set; } = new BotConfig();
        #endregion

        [JsonConstructor]
        private Config()
        {
            PathToConfig = DefaultConfigPath;
        }

        private Config(string pathToConfig)
        {
            if (string.IsNullOrEmpty(pathToConfig))
                throw new ArgumentNullException(nameof(pathToConfig));

            PathToConfig = pathToConfig;
        }

        ~Config()
        {
        }

        public static Config LoadOrCreateDefault(string pathToConfig)
        {
            if (string.IsNullOrEmpty(pathToConfig))
                throw new ArgumentNullException(nameof(pathToConfig));

            if (!File.Exists(pathToConfig))
                return SaveDefault(pathToConfig);

            return Load(pathToConfig);
        }

        public static Config Load(string pathToConfig)
        {
            if (string.IsNullOrEmpty(pathToConfig))
                throw new ArgumentNullException(nameof(pathToConfig));

            Config config;
            if (!File.Exists(pathToConfig))
                throw new ArgumentException($"No config file found: \"{pathToConfig}\"!");

            string json = File.ReadAllText(pathToConfig);
            config = JsonConvert.DeserializeObject<Config>(json) ?? throw new InvalidOperationException();
            config.PathToConfig = pathToConfig;

            // TODO: initialize default values, migrations and overrides here if required.

            config.Save(); // Re-save to add new or missing config fields.

            return config;
        }

        public static Config SaveDefault()
        {
            return SaveDefault(DefaultConfigPath);
        }

        public static Config SaveDefault(string pathToConfig)
        {
            Config config = new Config(pathToConfig);
            config.Save();

            return config;
        }

        public Config Save()
        {
            string json = ToJson();
            File.WriteAllText(PathToConfig, json);

            return this;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Formatting = Formatting.Indented, });
        }
    }
}
