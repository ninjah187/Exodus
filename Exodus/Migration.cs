using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus
{
    public class Migration
    {
        public int Version { get; }
        public string Name { get; }
        public string Script { get; }

        public Migration(int version, string name, string script)
        {
            Version = version;
            Name = name;
            Script = script;
        }
    }
}
