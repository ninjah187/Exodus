using Exodus.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Parsers
{
    public interface IAssemblyParser
    {
        IEnumerable<Task<Migration>> Parse(AssemblyName assemblyName);
    }
}
