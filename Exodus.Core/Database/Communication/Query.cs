using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Core.Database.Communication
{
    public abstract class Query<TConnection, TCommand, TParameters, TResult> : Message<TConnection, TCommand, TParameters>
        where TConnection : IDbConnection
        where TCommand : IDbCommand
        where TParameters : IDataParameterCollection
    {
        public Query(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<TResult> Execute()
        {
            Validate();
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection))
            {
                await OpenConnection(connection);
                return await ExecuteQuery(command);
            }
        }

        protected abstract Task<TResult> ExecuteQuery(TCommand command);
    }
}
