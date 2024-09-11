using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Commands
{
    internal interface IGlobalCommand : ICommand
    {
        public RestGlobalCommand RestGlobalCommand { get; set; }
    }
}
