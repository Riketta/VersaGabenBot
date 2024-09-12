using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    internal class DatabaseConfig : IOptions
    {
        public string ConnectionString { get; set; } =
            $"Data Source={Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{nameof(VersaGabenBot)}.db")}";
    }
}
