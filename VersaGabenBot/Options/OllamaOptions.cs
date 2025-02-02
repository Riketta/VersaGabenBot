namespace VersaGabenBot.Options
{
    internal class OllamaOptions : IOptions
    {
        public string OllamaApiUrl { get; set; } = "http://localhost:11434/api/";
        public int HttpClientTimeout { get; set; } = 210;
    }
}
