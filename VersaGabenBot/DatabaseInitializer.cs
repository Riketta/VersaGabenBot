﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Contexts;
using VersaGabenBot.Data.Models;
using VersaGabenBot.Options;

namespace VersaGabenBot
{
    internal class DatabaseInitializer
    {
        private readonly IDatabaseContext _context;

        public DatabaseInitializer(IDatabaseContext context)
        {
            _context = context;
        }

        public async void Initialize()
        {
            using var connection = await _context.GetConnection();

            CreateGuildsTable(connection);
            CreateChannelsTable(connection);
            CreateMessagesTable(connection);
            CreateUsersTable(connection);
            CreateGuildOptionsTable(connection);
            CreateChannelLlmOptionsTable(connection);
        }

        private async void CreateGuildsTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(Guild)}s (
                    {nameof(Guild.GuildID)} BIGINT PRIMARY KEY,
                    {nameof(Guild.SystemChannelID)} BIGINT
               );";

            await connection.ExecuteAsync(sql);
        }

        private async void CreateChannelsTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(Channel)}s (
                    {nameof(Channel.ChannelID)} BIGINT PRIMARY KEY,
                    {nameof(Channel.GuildID)} BIGINT REFERENCES {nameof(Guild)}s({nameof(Guild.GuildID)}) ON DELETE CASCADE,
                    {nameof(Channel.MessagesCutoff)} DATETIME NOT NULL
               );";

            await connection.ExecuteAsync(sql);
        }

        private async void CreateMessagesTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(Message)}s (
                    {nameof(Message.MessageID)} BIGINT PRIMARY KEY,
                    {nameof(Message.ChannelID)} BIGINT REFERENCES {nameof(Channel)}s({nameof(Channel.ChannelID)}) ON DELETE CASCADE,
                    {nameof(Message.UserID)} BIGINT NOT NULL,
                    {nameof(Message.Username)} VARCHAR(255) NOT NULL,
                    {nameof(Message.Timestamp)} DATETIME NOT NULL,
                    {nameof(Message.LlmRole)} INT NOT NULL,
                    {nameof(Message.BotRelated)} Boolean NOT NULL,
                    {nameof(Message.Content)} TEXT NOT NULL
               );";

            await connection.ExecuteAsync(sql);
        }

        private async void CreateUsersTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(User)}s (
                    {nameof(User.UserID)} BIGINT PRIMARY KEY,
                    {nameof(User.Username)} VARCHAR(255) NOT NULL,
                    {nameof(User.GlobalName)} VARCHAR(255)
               );";

            await connection.ExecuteAsync(sql);
        }

        private async void CreateGuildOptionsTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(GuildOptions)} (
                    {nameof(GuildOptions.GuildID)} BIGINT PRIMARY KEY REFERENCES {nameof(Guild)}s({nameof(Guild.GuildID)}) ON DELETE CASCADE,
                    {nameof(GuildOptions.MessageHistoryLimitPerChannel)} INT NOT NULL DEFAULT 300
               );";

            await connection.ExecuteAsync(sql);
        }

        private async void CreateChannelLlmOptionsTable(IDbConnection connection)
        {
            string sql =
                @$"CREATE TABLE IF NOT EXISTS {nameof(ChannelLlmOptions)} (
                    {nameof(ChannelLlmOptions.ChannelID)} BIGINT PRIMARY KEY REFERENCES {nameof(Channel)}s({nameof(Channel.ChannelID)}) ON DELETE CASCADE,
                    {nameof(ChannelLlmOptions.MessagesContextSize)} INT NOT NULL,
                    {nameof(ChannelLlmOptions.OnlyProcessChatHistoryRelatedToBot)} INT NOT NULL,
                    {nameof(ChannelLlmOptions.RandomReplyChance)} DOUBLE NOT NULL,
                    {nameof(ChannelLlmOptions.Model)} TEXT NOT NULL,
                    {nameof(ChannelLlmOptions.SystemPrompt)} TEXT
               );";

            await connection.ExecuteAsync(sql);
        }
    }
}
