using System.Linq;
using VersaGabenBot.Options;

namespace VersaGabenBot.Guilds
{
    internal class GuildManager
    {
        private readonly GuildOptions _storage;

        public GuildManager(GuildOptions guildOptions) // TODO: do not use config as storage.
        {
            _storage = guildOptions;
        }

        public Guild GetGuildByUUID(ulong uuid)
        {
            return _storage.Guilds.FirstOrDefault(guild => guild.ID == uuid);
        }

        public Guild GetGuildByChannelUUID(ulong uuid)
        {
            return _storage.Guilds.FirstOrDefault(guild => guild.ChannelIDs.Contains(uuid));
        }

        public Guild RegisterGuild(ulong guildID)
        {
            Guild guild = _storage.Guilds.FirstOrDefault(guild => guild.ID == guildID);
            if (guild is not null)
                return guild;

            guild = new Guild(guildID, _storage.DefaultMessageHistoryLimitPerChannel);
            _storage.Guilds.Add(guild);

            return guild;
        }
    }
}
