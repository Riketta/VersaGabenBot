﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot.Contexts
{
    internal interface IDatabase
    {
        public Task Save();
    }
}