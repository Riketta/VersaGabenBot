using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.Options;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using VersaGabenBot.LLM;
using System.Linq;

namespace VersaGabenBot.Ollama
{
    internal class OllamaClient : ILlmClient
    {
        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)
            }
        };

        private readonly OllamaOptions _options;
        private readonly HttpClient _httpClient;

        public OllamaClient(OllamaOptions options)
        {
            _options = options;

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_options.OllamaApiUrl),
                Timeout = new TimeSpan(0, 0, 0, _options.HttpClientTimeout, 0),
            };
        }

        public async Task<LlmMessage> GenerateTextAsync(string message)
        {
            return await GenerateTextAsync(new LlmMessage(Roles.User, message));
        }

        public async Task<LlmMessage> GenerateTextAsync(LlmMessage message)
        {
            return await GenerateTextAsync([message]);
        }

        public async Task<LlmMessage> GenerateTextAsync(IEnumerable<LlmMessage> messages)
        {
            ChatRequest chatRequest = new ChatRequest()
            {
                Model = _options.Model,
                Messages = new List<LlmMessage>(_options.SetupMessages),
                ModelfileOptions = new ModelfileOptions()
                {
                    ContextWindow = _options.ContextWindow,
                    Temperature = _options.Temperature
                },
                Stream = _options.Stream,
                KeepAlive = _options.KeepAlive,
            };
            chatRequest.Messages.AddRange(messages);

            string jsonRequest = JsonSerializer.Serialize(chatRequest, serializerOptions);

            var response = await _httpClient.PostAsync($"chat", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to generate text. Request: {response.RequestMessage.RequestUri}; Status code: {response.StatusCode}; Body: {responseJson}.");

            var responseData = JsonNode.Parse(responseJson);
            LlmMessage llmResponse = JsonSerializer.Deserialize<LlmMessage>(responseData["message"], serializerOptions) ?? throw new InvalidOperationException();

            return llmResponse;
        }
    }
}
