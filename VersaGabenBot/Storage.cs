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
using VersaGabenBot.Guilds;

namespace VersaGabenBot
{
    internal class Storage
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

        #region Storage Fields
        public HashSet<Guild> Guilds { get; set; } = new HashSet<Guild>();
        #endregion

        public async static Task<Storage> Load()
        {
            Storage storage;
            bool freshStorage = !File.Exists(DefaultStoragePath);

            if (freshStorage)
                storage = new Storage();
            else
                using (var fileStream = new FileStream(DefaultStoragePath, FileMode.OpenOrCreate, FileAccess.Read))
                    storage = await JsonSerializer.DeserializeAsync<Storage>(fileStream, serializerOptions) ?? throw new InvalidOperationException();

            // TODO: initialize default values, migrations and overrides here if required.

            await storage.Save(); // Re-save to add new or missing fields.

            return storage;
        }

        public async Task Save()
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, this, GetType(), options: serializerOptions);
            stream.Position = 0;

            using var fileStream = new FileStream(DefaultStoragePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
        }
    }
}
