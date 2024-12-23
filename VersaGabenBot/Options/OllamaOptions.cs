﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersaGabenBot.LLM;

namespace VersaGabenBot.Options
{
    internal class OllamaOptions : IOptions
    {
        public string OllamaApiUrl { get; set; } = "http://localhost:11434/api/";
        public int HttpClientTimeout { get; set; } = 210;
        public string Model { get; set; } = "phi3.5";
        public bool Multimodal { get; set; } = false;
        public uint ContextWindow { get; set; } = 16384;
        public string KeepAlive { get; set; } = "-1m";
        public bool Stream { get; set; } = false;
        public float Temperature { get; set; } = 0.8f;
    }
}
