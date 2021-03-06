﻿using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Core.Parsers
{
    public interface IDirectoryParser
    {
        IEnumerable<Task<Migration>> Parse(string directoryPath);
    }
}
