﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Data.Models
{
    internal class Channel
    {
        public ulong ChannelID { get; set; }
        public ulong GuildID { get; set; }
        //public Guild Guild { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}