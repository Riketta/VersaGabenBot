using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VersaGabenBot.Ollama
{
    /// <summary>
    /// Details: https://github.com/ollama/ollama/blob/main/docs/modelfile.md.
    /// </summary>
    internal class ModelfileOptions
    {
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.8f;

        [JsonPropertyName("num_ctx")]
        public uint ContextWindow { get; set; } = 2048;
    }
}
