using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VersaGabenBot.LLM;
using VersaGabenBot.Options;

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

        public async Task<LlmMessage> GenerateTextAsync(Model model, string message)
        {
            return await GenerateTextAsync(model, new LlmMessage(Roles.User, message));
        }

        public async Task<LlmMessage> GenerateTextAsync(Model model, LlmMessage message)
        {
            return await GenerateTextAsync(model, [message]);
        }

        public async Task<LlmMessage> GenerateTextAsync(Model model, IEnumerable<LlmMessage> messages)
        {
            return await GenerateTextAsync(model, messages, null);
        }

        public async Task<LlmMessage> GenerateTextAsync(Model model, IEnumerable<LlmMessage> messages, string systemPrompt)
        {

            ChatRequest chatRequest = new ChatRequest()
            {
                Model = model.Name,
                Messages = new List<LlmMessage>(),
                Parameters = model.Parameters,
                Stream = model.Stream,
                KeepAlive = model.KeepAlive,
            };

            if (!string.IsNullOrEmpty(systemPrompt))
                chatRequest.Messages.Add(new LlmMessage(Roles.System, systemPrompt));
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
