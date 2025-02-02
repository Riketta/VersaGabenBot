namespace VersaGabenBot.Options
{
    internal class OllamaOptions : IOptions
    {
        public string OllamaApiUrl { get; set; } = "http://localhost:11434/api/";
        public int HttpClientTimeout { get; set; } = 210;
        public string Model { get; set; } = "phi3.5";
        public bool Multimodal { get; set; } = false;
        public uint ContextWindow { get; set; } = 8192;
        public int TopK { get; set; } = 40;
        public float MinP { get; set; } = 0.05f;
        public float TopP { get; set; } = 0.95f;
        public string KeepAlive { get; set; } = "-1m";
        public bool Stream { get; set; } = false;
        public float Temperature { get; set; } = 0.8f;
    }
}
