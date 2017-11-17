using Jarilo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Npgsql
{
    class Arguments
    {
        [Argument("Database connection string.")]
        public string ConnectionString { get; set; }

        [Argument("Migrations source directory path.")]
        public string Directory { get; set; } = ".";
    }
}
