using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VersaGabenBot.Ollama
{
    [JsonConverter(typeof(JsonStringEnumConverter<Roles>))]
    internal enum Roles
    {
        System,
        User,
        Assistant
    }
}
