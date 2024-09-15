using Microsoft.Data.Sqlite;
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
        public static async Task<IDbConnection> CreateInMemoryDatabase(SqliteConnection conn)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            return connection;
        }
    }
}
