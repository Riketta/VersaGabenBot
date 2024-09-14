using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;

namespace VersaGabenBot
{
    internal class TemplateEmbedBuilder
    {
        public static EmbedBuilder Error(string message)
        {
            var embed = new EmbedBuilder()
            .WithTitle("Error")
            .WithDescription(message)
            .WithColor(Color.Red)
            .WithCurrentTimestamp();

            return embed;
        }

        public static EmbedBuilder ErrorUnauthorizedCommandUse()
        {
            return Error("Unauthorized use of command!");
        }

        public static EmbedBuilder ErrorGuildNotRegistered()
        {
            return Error("Guild not registered!");
        }

        public static EmbedBuilder ErrorChannelNotRegistered()
        {
            return Error("Channel not registered!");
        }
    }
}
