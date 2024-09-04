﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Guilds;

namespace VersaGabenBot.Options
{
    internal class GuildOptions : IOptions
    {
        public LlmOptionsGuild LlmOptions { get; set; } = new LlmOptionsGuild();
        public uint MessageHistoryLimitPerChannel { get; set; } = 300;
    }
}
