using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.SqlServer
{
    class ViewModel
    {
        public string[] Logs { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
