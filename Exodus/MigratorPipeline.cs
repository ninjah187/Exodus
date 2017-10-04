using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exodus
{
    class MigratorPipeline
    {
        public List<Func<Task>> Setup { get; }
        public List<Func<Task>> Migrations { get; }

        public MigratorPipeline()
        {
            Setup = new List<Func<Task>>();
            Migrations = new List<Func<Task>>();
        }
    }
}
