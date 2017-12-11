using Jarilo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exodus.Cli.Models
{
    enum DatabaseSetupOperation
    {
        None,

        [Value("create", "Create database if not exists.")]
        CreateIfNotExists,

        [Value("drop-create", "Drop and create database always.")]
        DropCreate
    }
}
