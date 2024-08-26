using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    public class BotConfig : IOptions
    {
        public string Token { get; set; }
        public ulong GuildID { get; set; }
        public ulong GeneralChannelID { get; set; }
        public ulong BotChannelID { get; set; }
        public uint StatusUpdateInterval { get; set; } = 60 * 60 * 1000;
        public List<string> StatusList { get; set; }
        public double RandomReplyChance { get; set; } = 0.01f;
    }
}
