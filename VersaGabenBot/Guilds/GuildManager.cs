using System.Linq;
using System.Threading;
using VersaGabenBot.Options;

namespace VersaGabenBot.Guilds
{
    internal class GuildManager
    {
        private readonly GuildOptions _options;
        private readonly Storage _storage; // TODO: turn to property and add lock?
        private readonly Timer _timer;

        public GuildManager(GuildOptions guildOptions) // TODO: do not use config as storage.
        {
            _options = guildOptions;
            _storage = Storage.Load().Result;
            _timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: null,
                dueTime: _options.DatabaseSaveInterval,
                period: _options.DatabaseSaveInterval);
        }

        private async void TimerTask(object timerState)
        {
            await _storage.Save();
        }

        public Guild GetGuildByUUID(ulong uuid)
        {
            return _storage.Guilds.FirstOrDefault(guild => guild.ID == uuid);
        }

        public Guild GetGuildByChannelUUID(ulong uuid)
        {
            return _storage.Guilds.FirstOrDefault(guild => guild.IsChannelRegistered(uuid));
        }

        public Guild RegisterGuild(ulong guildID)
        {
            Guild guild = _storage.Guilds.FirstOrDefault(guild => guild.ID == guildID);
            if (guild is not null)
                return guild;

            guild = new Guild(guildID, _options.DefaultMessageHistoryLimitPerChannel);
            _storage.Guilds.Add(guild);

            return guild;
        }
    }
}
