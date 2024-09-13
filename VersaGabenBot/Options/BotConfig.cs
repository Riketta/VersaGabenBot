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
        public ulong DebugUserID { get; set; }
        public int MaxMessageLength { get; set; } = 2000;
        public uint StatusUpdateInterval { get; set; } = 60 * 60 * 1000;
        public List<string> StatusList { get; set; }
    }
}
