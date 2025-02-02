using System.Collections.Generic;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Options
{
    internal class ModelOptions : IOptions
    {
        public List<Model> Models { get; set; } =
        [
            new Model("gemma2")
            {
                Parameters = new Dictionary<string, object>()
                {
                    ["temperature"] = 1.0f,
                    ["num_ctx"] = 8192,
                }
            },
            new Model("aya-expanse")
            {
                Parameters = new Dictionary<string, object>()
                {
                    ["temperature"] = 1.0f,
                    ["num_ctx"] = 12288,
                    ["top_k"] = 50,
                    ["top_p"] = 0.95f,
                    ["min_p"] = 0.05f,
                }
            }
        ];
    }
}
