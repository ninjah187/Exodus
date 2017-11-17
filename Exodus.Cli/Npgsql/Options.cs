using Exodus.Cli.Models;
using Jarilo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Npgsql
{
    class Options
    {
        [Option("--setup", "Optional database setup operation.")]
        public DatabaseSetupOperation SetupOperation { get; set; } = DatabaseSetupOperation.None;
    }
}
