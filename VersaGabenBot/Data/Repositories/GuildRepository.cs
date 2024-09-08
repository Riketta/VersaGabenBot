﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Repositories
{
    internal class GuildRepository
    {
        private readonly Database _db;

        public GuildRepository(Database database)
        {
            _db = database;
        }

        public Guild GetGuildByUUID(ulong uuid)
        {
            return _db.Guilds.FirstOrDefault(guild => guild.ID == uuid);
        }

        public Guild GetGuildByChannelUUID(ulong uuid)
        {
            return _db.Guilds.FirstOrDefault(guild => guild.IsChannelRegistered(uuid));
        }

        public Guild RegisterGuild(ulong guildID)
        {
            Guild guild = _db.Guilds.FirstOrDefault(guild => guild.ID == guildID);
            if (guild is not null)
                return guild;

            guild = new Guild(guildID);
            _db.Guilds.Add(guild);

            return guild;
        }
    }
}