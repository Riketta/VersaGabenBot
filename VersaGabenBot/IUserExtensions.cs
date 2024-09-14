using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Data.Models;

namespace VersaGabenBot
{
    internal static class IUserExtensions
    {
        public static string GetGlobalNameOrUsername(this IUser user)
        {
            return user.GlobalName ?? user.Username;
        }
    }
}
