using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    public interface IAssemblyParser
    {
        IEnumerable<Task<Migration>> Parse(AssemblyName assemblyName);
    }
}
