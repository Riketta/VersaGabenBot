﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Data.Models
{
    internal class Message
    {
        public ulong MessageID { get; set; }
        public ulong ChannelID { get; set; }
        //public Channel Channel { get; set; }
        public ulong UserID { get; set; }
        public string Username { get; set; } // TODO: temp, use Users table.
        public DateTime Timestamp { get; set; }
        public Roles LlmRole { get; set; }
        public bool BotRelated { get; set; }
        public string Content { get; set; }

        public Message() { }

        public Message(IUserMessage message, Roles llmRole, bool botRelated)
        {
            MessageID = message.Id;
            ChannelID = message.Channel.Id;
            UserID = message.Author.Id;
            Username = message.Author.GetGlobalNameOrUsername();
            Timestamp = message.Timestamp.DateTime;
            BotRelated = botRelated;
            Content = message.CleanContent;

            LlmRole = llmRole;
        }
    }
}
