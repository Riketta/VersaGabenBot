using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;
using System;

namespace VersaGabenBot.Ollama
{
    internal class OllamaClient
    {
        private readonly OllamaOptions _options;
        private readonly HttpClient _httpClient;

        public OllamaClient(OllamaOptions options)
        {
            _options = options;

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_options.OllamaApiUrl),
            };
        }

        public async Task<string> GenerateTextAsync(string message, int maxTokens = 8)
        {
            var requestData = new
            {
                messages = new[]
                {
                    new { role = "user", content = message }
                },
                max_tokens = maxTokens
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
            });

            var response = await _httpClient.PostAsync($"/chat", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to generate text. Status code: {response.StatusCode}, Body: {responseJson}");

            var responseData = JObject.Parse(responseJson);
            return responseData["choices"][0]["message"]["content"].ToString();
        }
    }
}
