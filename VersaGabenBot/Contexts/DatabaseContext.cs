using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Threading.Tasks;
using VersaGabenBot.Options;

namespace VersaGabenBot.Contexts
{
    internal class DatabaseContext
    {
        private readonly DatabaseConfig _config;

        public DatabaseContext(DatabaseConfig config)
        {
            _config = config;
        }

        public async Task<IDbConnection> GetConnection()
        {
            var connection = new SqliteConnection(_config.ConnectionString);
            await connection.OpenAsync();

            return connection;
        }

        public async Task<int> ExecuteQueryAsync(string sql, object parameters = null)
        {
            using var connection = await GetConnection();

            return await connection.ExecuteAsync(sql, parameters);
        }

        public async Task Save()
        {
            throw new NotImplementedException();
        }
    }
}
