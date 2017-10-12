using Npgsql;
using System;

namespace Exodus.Npgsql
{
    class Message
    {
        protected string ConnectionString { get; }
        protected string Sql { get; set; }

        public Message(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected virtual void AddParameters(NpgsqlParameterCollection parameters)
        {
        }

        protected virtual void Validate()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentException("Connection string is not defined.", nameof(ConnectionString));
            }
            if (string.IsNullOrEmpty(Sql))
            {
                throw new ArgumentException("SQL statement is not defined.", nameof(Sql));
            }
        }
    }
}
