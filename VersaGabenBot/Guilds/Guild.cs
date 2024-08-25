using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Guilds
{
    internal class Guild
    {
        [JsonProperty]
        public ulong GuildID { get; private set; }

        [JsonProperty]
        public ulong GeneralChannelID { get; set; }

        [JsonProperty]
        public ulong BotChannelID { get; set; }
    }
}
