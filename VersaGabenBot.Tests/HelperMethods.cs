﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Tests
{
    internal class HelperMethods
    {
        public static async Task<IDbConnection> CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            return connection;
        }

        public static async Task<IDbConnection> CreateTemporaryDatabase()
        {
            var connection = new SqliteConnection("Data Source=Tests.db");
            await connection.OpenAsync();

            return connection;
        }
    }
}