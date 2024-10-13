using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Options
{
    public class ProxyOptions : IOptions
    {
        public bool WebProxyEnabled { get; set; } = false;
        public string Address { get; set; } = "socks5://127.0.0.1:1080";
        public bool BypassOnLocal { get; set; } = false;
        public string[] BypassList { get; set; } = [];
        public bool UseCredentials { get; set; } = false;
        public NetworkCredential Credentials { get; set; } = new NetworkCredential();
    }
}
