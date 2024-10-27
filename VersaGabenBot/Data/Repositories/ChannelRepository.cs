using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot.Data.Repositories
{
    internal class ChannelRepository
    {
        private readonly IDatabaseContext _db;

        public ChannelRepository(IDatabaseContext database)
        {
            _db = database;
        }

        #region Channel
        public async Task<Channel> RegisterChannel(ulong channelId, ulong guildId)
        {
            Channel channel = new Channel()
            {
                ChannelID = channelId,
                GuildID = guildId,
                LlmOptions = new ChannelLlmOptions(channelId),
            };

            var sql =
                @$"INSERT INTO {nameof(Channel)}s (
                    {nameof(Channel.ChannelID)},
                    {nameof(Channel.GuildID)},
                    {nameof(Channel.MessagesCutoff)}
                ) VALUES (
                    @{nameof(channelId)},
                    @{nameof(guildId)},
                    @{nameof(Channel.MessagesCutoff)}
                );";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    channelId,
                    guildId,
                    channel.MessagesCutoff,
                }).ConfigureAwait(false);

            await InsertChannelLlmOptions(channel.LlmOptions);

            return channel;
        }

        public async Task UnregisterChannel(ulong channelId)
        {
            var sql =
                @$"DELETE FROM {nameof(Channel)}s
                WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)}";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { channelId });

            // TODO: remove according options.
        }

        public async Task<Channel> GetChannel(ulong channelId)
        {
            var sql =
                @$"SELECT
                    C.*,
                    CLO.*
                FROM
                    {nameof(Channel)}s AS C
                LEFT JOIN
                    {nameof(ChannelLlmOptions)} AS CLO ON C.{nameof(Channel.ChannelID)} = CLO.{nameof(ChannelLlmOptions.ChannelID)}
                WHERE
                    C.{nameof(Channel.ChannelID)} = @{nameof(channelId)};";

            using var connection = await _db.GetConnection();
            var channel = (await connection.QueryAsync<Channel, ChannelLlmOptions, Channel>(
            sql,
            (channel, llmOptions) =>
            {
                channel.LlmOptions = llmOptions;

                return channel;
            },
            new { channelId },
            splitOn: nameof(Channel.ChannelID)
        )).SingleOrDefault();

            return channel;
        }

        public async Task<Channel> GetChannelWithMessages(ulong channelId)
        {
            var sql =
                @$"SELECT * FROM {nameof(Channel)}s
                WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)};

                SELECT * FROM {nameof(Message)}s
                WHERE {nameof(Message.ChannelID)} = @{nameof(channelId)};";

            using var connection = await _db.GetConnection();
            var mapper = await connection.QueryMultipleAsync(sql, new { channelId });
            var channelWithMessages = await mapper.ReadSingleOrDefaultAsync<Channel>();

            if (channelWithMessages is null)
                return null;

            var channelMessages = await mapper.ReadAsync<Message>();
            channelWithMessages.Messages = channelMessages.ToList();

            return channelWithMessages;
        }

        public async Task<bool> IsChannelRegistered(ulong channelId)
        {
            var sql =
                @$"SELECT COUNT(*) FROM {nameof(Channel)}s
                WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)};";

            using var connection = await _db.GetConnection();
            var registered = await connection.ExecuteScalarAsync<int>(sql, new { channelId }) == 1;

            return registered;
        }

        public async Task<bool> UpdateChannelCutoff(ulong channelId, DateTime newCutoff)
        {
            var sql =
                @$"UPDATE {nameof(Channel)}s
                SET {nameof(Channel.MessagesCutoff)} = @{nameof(newCutoff)}
                WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)};";

            using var connection = await _db.GetConnection();
            bool success = await connection.ExecuteScalarAsync<int>(sql, new { channelId, newCutoff }) == 1;

            return success;
        }
        #endregion

        #region Message
        public async Task InsertMessage(Message message)
        {
            var sql =
                @$"INSERT INTO {nameof(Message)}s (
                    {nameof(Message.MessageID)},
                    {nameof(Message.ChannelID)},
                    {nameof(Message.UserID)},
                    {nameof(Message.Timestamp)},
                    {nameof(Message.Username)},
                    {nameof(Message.LlmRole)},
                    {nameof(Message.BotRelated)},
                    {nameof(Message.Content)}
                ) VALUES (
                    @{nameof(Message.MessageID)},
                    @{nameof(Message.ChannelID)},
                    @{nameof(Message.UserID)},
                    @{nameof(Message.Timestamp)},
                    @{nameof(Message.Username)},
                    @{nameof(Message.LlmRole)},
                    @{nameof(Message.BotRelated)},
                    @{nameof(Message.Content)}
                );";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    message.MessageID,
                    message.ChannelID,
                    message.UserID,
                    message.Timestamp,
                    message.Username,
                    message.LlmRole,
                    message.BotRelated,
                    message.Content,
                });
        }

        public async Task<List<Message>> GetMessages(ulong channelId, uint count)
        {
            var sql =
                @$"SELECT * FROM (
                    SELECT * FROM {nameof(Message)}s
                    WHERE {nameof(Message.ChannelID)} = @{nameof(channelId)}
                    ORDER BY {nameof(Message.Timestamp)} DESC
                    LIMIT @{nameof(count)}
                    )
                ORDER BY Timestamp ASC;";

            using var connection = await _db.GetConnection();
            var messages = await connection.QueryAsync<Message>(sql, new { channelId, count });

            return messages.ToList();
        }

        public async Task<List<Message>> GetMessagesWithCutoff(ulong channelId, uint count)
        {
            var sql =
                @$"SELECT * FROM (
	                SELECT * FROM {nameof(Message)}s as M
	                LEFT JOIN (
		                SELECT {nameof(Channel.MessagesCutoff)} FROM {nameof(Channel)}s WHERE {nameof(Channel.ChannelID)} = @{nameof(channelId)}
	                ) AS C
	                WHERE M.{nameof(Message.ChannelID)} = @{nameof(channelId)} AND M.{nameof(Message.Timestamp)} > C.{nameof(Channel.MessagesCutoff)}
	                ORDER BY M.{nameof(Message.Timestamp)} DESC
	                LIMIT @{nameof(count)}
	                )
                ORDER BY Timestamp ASC;";

            using var connection = await _db.GetConnection();
            var messages = await connection.QueryAsync<Message>(sql, new { channelId, count });

            return messages.ToList();
        }

        public async Task<uint> GetMessagesCount(ulong channelId)
        {
            var sql =
                @$"SELECT COUNT(*) FROM {nameof(Message)}s
                WHERE {nameof(Message.ChannelID)} = @{nameof(channelId)}";

            using var connection = await _db.GetConnection();
            var messages = await connection.ExecuteScalarAsync<uint>(sql, new { channelId });

            return messages;
        }

        public async Task DeleteMessage(ulong messageId)
        {
            var sql =
                @$"DELETE FROM {nameof(Message)}s
                WHERE {nameof(Message.MessageID)} = @{nameof(messageId)}";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { messageId });
        }

        public async Task<uint> DeleteAllMessages(ulong channelId)
        {
            var sql =
                @$"DELETE FROM {nameof(Message)}s
                WHERE {nameof(Message.ChannelID)} = @{nameof(channelId)}
                RETURNING COUNT(*)";

            using var connection = await _db.GetConnection();
            var rowsAffected = await connection.ExecuteScalarAsync<uint>(sql, new { channelId });

            return rowsAffected;
        }

        public async Task InsertChannelLlmOptions(ChannelLlmOptions options)
        {
            var sql =
                @$"INSERT INTO {nameof(ChannelLlmOptions)} (
                    {nameof(ChannelLlmOptions.ChannelID)},
                    {nameof(ChannelLlmOptions.MessagesContextSize)},
                    {nameof(ChannelLlmOptions.OnlyProcessChatHistoryRelatedToBot)},
                    {nameof(ChannelLlmOptions.RandomReplyChance)}
                ) VALUES (
                    @{nameof(options.ChannelID)},
                    @{nameof(ChannelLlmOptions.MessagesContextSize)},
                    @{nameof(ChannelLlmOptions.OnlyProcessChatHistoryRelatedToBot)},
                    @{nameof(ChannelLlmOptions.RandomReplyChance)}
                );";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    options.ChannelID,
                    options.MessagesContextSize,
                    options.OnlyProcessChatHistoryRelatedToBot,
                    options.RandomReplyChance,
                });
        }
        #endregion

        #region ChannelLlmOptions
        public async Task<ChannelLlmOptions> GetChannelLlmOptions(ulong channelId)
        {
            var sql =
                @$"SELECT * FROM {nameof(ChannelLlmOptions)}
                WHERE {nameof(ChannelLlmOptions.ChannelID)} = @{nameof(channelId)};";

            using var connection = await _db.GetConnection();
            var options = await connection.QuerySingleOrDefaultAsync<ChannelLlmOptions>(sql, new { channelId });

            return options;
        }

        public async Task UpdateChannelLlmOptions(ChannelLlmOptions options)
        {
            var sql =
                @$"UPDATE
                    {nameof(ChannelLlmOptions)}
                SET
                    {nameof(ChannelLlmOptions.MessagesContextSize)} = @{nameof(options.MessagesContextSize)},
                    {nameof(ChannelLlmOptions.OnlyProcessChatHistoryRelatedToBot)} = @{nameof(options.OnlyProcessChatHistoryRelatedToBot)},
                    {nameof(ChannelLlmOptions.RandomReplyChance)} = @{nameof(options.RandomReplyChance)}
                WHERE
                    {nameof(ChannelLlmOptions.ChannelID)} = @{nameof(options.ChannelID)};";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql,
                new
                {
                    options.MessagesContextSize,
                    options.OnlyProcessChatHistoryRelatedToBot,
                    options.RandomReplyChance,
                    options.ChannelID
                });
        }

        public async Task DeleteChannelLlmOptions(ulong channelId)
        {
            var sql =
                @$"DELETE FROM {nameof(ChannelLlmOptions)}
                WHERE {nameof(ChannelLlmOptions.ChannelID)} = @{nameof(channelId)}";

            using var connection = await _db.GetConnection();
            await connection.ExecuteAsync(sql, new { channelId });
        }
        #endregion
    }
}
