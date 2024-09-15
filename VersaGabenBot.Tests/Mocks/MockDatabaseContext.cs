using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Contexts;

namespace VersaGabenBot.Tests.Mocks
{
    internal class MockDatabaseContext : IDatabaseContext
    {
        public MockDatabaseContext()
        {
        }

        public async Task<IDbConnection> GetConnection()
        {
            IDbConnection connection = await HelperMethods.CreateTemporaryDatabase();
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

        public void Wipe()
        {
            HelperMethods.RemoveTemporaryDatabase();
        }
    }
}
