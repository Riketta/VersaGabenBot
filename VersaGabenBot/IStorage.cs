﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot
{
    internal interface IStorage
    {
        public Task Save();
    }
}