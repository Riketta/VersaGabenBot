using System.Data;
using System.Threading.Tasks;

namespace VersaGabenBot.Contexts
{
    internal interface IDatabaseContext
    {
        Task<int> ExecuteQueryAsync(string sql, object parameters = null);
        Task<IDbConnection> GetConnection();
        Task Save();
    }
}