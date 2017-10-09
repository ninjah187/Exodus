using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Core
{
    class Pipeline
    {
        public List<Func<Task>> Setup { get; }
        public List<Func<Task>> Migrations { get; }

        public Pipeline()
        {
            Setup = new List<Func<Task>>();
            Migrations = new List<Func<Task>>();
        }
    }
}
