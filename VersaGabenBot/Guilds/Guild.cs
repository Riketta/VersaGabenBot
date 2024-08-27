﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Guilds
{
    internal class Guild
    {
        [JsonInclude]
        public List<string> BotNames { get; private set; }

        [JsonInclude]
        public ulong ID { get; private set; }

        [JsonInclude]
        public ulong BotChannelID { get; set; }

        [JsonInclude]
        private HashSet<ulong> ChannelIDs { get; set; } = new HashSet<ulong>();

        [JsonInclude]
        public uint MessageHistoryLimit { get; set; } = 100;

        [JsonInclude]
        public ConcurrentDictionary<ulong, ConcurrentQueue<Message>> MessageHistoryPerChannel { get; private set; } = new();

        [JsonConstructor]
        public Guild() { }

        public Guild(ulong id, uint messageHistoryLimit)
        {
            ID = id;
            MessageHistoryLimit = messageHistoryLimit;
        }

        public void AppendMessage(ulong channel, Message message)
        {
            if (!MessageHistoryPerChannel.ContainsKey(channel))
                MessageHistoryPerChannel[channel] = new ConcurrentQueue<Message>();

            MessageHistoryPerChannel[channel].Enqueue(message);

            if (MessageHistoryPerChannel[channel].Count > MessageHistoryLimit)
                MessageHistoryPerChannel[channel].TryDequeue(out Message _);
        }

        public void ClearChannelHistory(ulong channel)
        {
            if (!MessageHistoryPerChannel.ContainsKey(channel))
                return;

            MessageHistoryPerChannel[channel].Clear();
        }

        public void RegisterChannel(ulong channel)
        {
            ChannelIDs.Add(channel);
        }

        public void UnregisterChannel(ulong channel)
        {
            ChannelIDs.Remove(channel);
        }
    }
}
