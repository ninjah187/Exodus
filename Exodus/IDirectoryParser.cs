using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    public interface IDirectoryParser
    {
        IEnumerable<Task<Migration>> Parse(string directoryPath);
    }
}
