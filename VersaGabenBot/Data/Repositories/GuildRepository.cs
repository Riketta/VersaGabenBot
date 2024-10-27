using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Repositories
{
    internal class GuildRepository
    {
        private readonly IDatabaseContext _db;

        public GuildRepository(IDatabaseContext database)
        {
            _db = database;
        }

        #region Guild
        public async Task<Guild> RegisterGuild(ulong guildId)
        {
            Guild guild = new Guild()
            {
                GuildID = guildId,
                Options = new GuildOptions(guildId),
            };

            var sql =
                @$"INSERT INTO {nameof(Guild)}s ({nameof(Guild.GuildID)})
                VALUES (@{nameof(guildId)});";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { guildId }).ConfigureAwait(false);

            // TODO: rewrite as single request.
            await InsertGuildOptions(guild.Options);

            return guild;
        }

        public async Task UnregisterGuild(ulong guildId)
        {
            var sql =
                @$"DELETE FROM {nameof(Guild)}s
                WHERE {nameof(Guild.GuildID)} = @{nameof(guildId)};";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { guildId }).ConfigureAwait(false);

            // TODO: remove according options.
        }

        public async Task<Guild> GetGuild(ulong guildId)
        {
            var sql =
                @$"SELECT
                    G.*,
                    GO.*
                FROM
                    {nameof(Guild)}s AS G
                LEFT JOIN
                    {nameof(GuildOptions)} AS GO ON G.{nameof(Guild.GuildID)} = GO.{nameof(GuildOptions.GuildID)}
                WHERE
                    G.{nameof(Guild.GuildID)} = @{nameof(guildId)};";

            using var connection = await _db.GetConnection();
            var guild = (await connection.QueryAsync<Guild, GuildOptions, Guild>(
                sql,
                (guild, options) =>
                {
                    guild.Options = options;

                    return guild;
                },
                new { guildId },
                splitOn: nameof(Guild.GuildID)
            )).SingleOrDefault();

            return guild;
        }

        public async Task<Guild> GetGuildByChannelID(ulong channelId)
        {
            var sql =
                @$"SELECT {nameof(Guild.GuildID)}
                FROM {nameof(Guild)}s
                WHERE {nameof(Guild.GuildID)} = (
                    SELECT {nameof(Channel.GuildID)}
                    FROM {nameof(Channel)}s
                    WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)});";

            using var connection = await _db.GetConnection();
            var guildId = await connection.ExecuteScalarAsync<ulong>(sql, new { channelId });
            var guild = await GetGuild(guildId); // TODO: rewrite as single query.

            return guild;
        }

        public async Task<bool> IsGuildRegistered(ulong guildId)
        {
            var sql =
                @$"SELECT COUNT(*) FROM {nameof(Guild)}s
                WHERE {nameof(Guild.GuildID)} = @{nameof(guildId)};";

            using var connection = await _db.GetConnection();
            var registered = await connection.ExecuteScalarAsync<int>(sql, new { guildId }) == 1;

            return registered;
        }
        #endregion

        #region GuildOptions
        public async Task InsertGuildOptions(GuildOptions options)
        {
            var sql =
                @$"INSERT INTO {nameof(GuildOptions)} (
                    {nameof(GuildOptions.GuildID)},
                    {nameof(GuildOptions.MessageHistoryLimitPerChannel)}
                ) VALUES (
                    @{nameof(options.GuildID)},
                    @{nameof(options.MessageHistoryLimitPerChannel)}
                );";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { options.GuildID, options.MessageHistoryLimitPerChannel });
        }

        public async Task<GuildOptions> GetGuildOptions(ulong guildId)
        {
            var sql =
                @$"SELECT * FROM {nameof(GuildOptions)}
                WHERE {nameof(GuildOptions.GuildID)} = @{nameof(guildId)};";

            using var connection = await _db.GetConnection();
            var options = await connection.QuerySingleOrDefaultAsync<GuildOptions>(sql, new { guildId });

            return options;
        }

        public async Task UpdateGuildOptions(GuildOptions options)
        {
            var sql =
                @$"UPDATE {nameof(GuildOptions)}
                SET
                {nameof(GuildOptions.MessageHistoryLimitPerChannel)} = @{nameof(options.MessageHistoryLimitPerChannel)}
                WHERE {nameof(GuildOptions.GuildID)} = @{nameof(options.GuildID)};";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { options.MessageHistoryLimitPerChannel, options.GuildID });
        }

        public async Task DeleteGuildOptions(ulong guildId)
        {
            var sql =
                @$"DELETE FROM {nameof(GuildOptions)}
                WHERE {nameof(GuildOptions.GuildID)} = @{nameof(guildId)}";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { guildId });
        }
        #endregion
    }
}
