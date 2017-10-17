using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Core.Database.Communication
{
    public abstract class Message<TConnection, TCommand, TParameters>
        where TConnection : IDbConnection
        where TCommand : IDbCommand
        where TParameters : IDataParameterCollection
    {
        protected string ConnectionString { get; }
        protected string Sql { get; set; }

        public Message(string connectionString)
        {
            ConnectionString = connectionString;
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

        protected abstract TConnection CreateConnection();
        protected abstract TCommand CreateCommand(TConnection connection);
        protected abstract Task OpenConnection(TConnection connection);

        protected virtual void AddParameters(TParameters parameters)
        {
        }
    }
}
