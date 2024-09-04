using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using VersaGabenBot.Options;
using System.IO;
using VersaGabenBot.Data.Models;
using System.Threading;

namespace VersaGabenBot.Contexts
{
    internal class Database
    {
        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        public static readonly string DefaultStoragePath = $"{nameof(VersaGabenBot)}DB.json";

        public int DatabaseSaveInterval { get; set; } = 5 * 60 * 1000;

        #region Storage Fields
        public HashSet<Guild> Guilds { get; set; } = new HashSet<Guild>();
        #endregion

        private readonly Timer _timer;

        [JsonConstructor]
        private Database()
        {
            _timer = new Timer(
                callback: new TimerCallback(TimerTask),
                state: null,
                dueTime: DatabaseSaveInterval,
                period: DatabaseSaveInterval);
        }

        public async static Task<Database> Load()
        {
            Database database;
            bool freshStorage = !File.Exists(DefaultStoragePath);

            if (freshStorage)
                database = new Database();
            else
                using (var fileStream = new FileStream(DefaultStoragePath, FileMode.OpenOrCreate, FileAccess.Read))
                    database = await JsonSerializer.DeserializeAsync<Database>(fileStream, serializerOptions) ?? throw new InvalidOperationException();

            // TODO: initialize default values, migrations and overrides here if required.

            await database.Save(); // Re-save to add new or missing fields.

            return database;
        }

        public async Task Save()
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, this, GetType(), options: serializerOptions);
            stream.Position = 0;

            using var fileStream = new FileStream(DefaultStoragePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
        }

        private async void TimerTask(object timerState)
        {
            await SaveDatabase();
        }

        public async Task SaveDatabase()
        {
            await Save();
        }
    }
}
