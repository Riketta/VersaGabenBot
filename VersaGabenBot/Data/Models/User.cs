using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Data.Models
{
    internal class User
    {
        public ulong UserID { get; set; }
        public string Username { get; set; }
        public string GlobalName { get; set; }
    }
}
