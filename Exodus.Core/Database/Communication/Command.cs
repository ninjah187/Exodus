using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Exodus.Database.Communication
{
    public abstract class Command<TConnection, TCommand, TParameters> : Message<TConnection, TCommand, TParameters>
        where TConnection : IDbConnection
        where TCommand : IDbCommand
        where TParameters : IDataParameterCollection
    {
        readonly bool _isTransactional;

        public Command(string connectionString, bool isTransactional = true)
            : base(connectionString)
        {
            _isTransactional = isTransactional;
        }

        public async Task Execute()
        {
            Validate();
            using (var connection = CreateConnection())
            using (var command = CreateCommand(connection))
            {
                await OpenConnection(connection);
                AddParameters((TParameters) command.Parameters);
                if (_isTransactional)
                {
                    await ExecuteCommandInTransaction(command);
                }
                else
                {
                    await ExecuteCommand(command);
                }
            }
        }

        protected abstract Task ExecuteCommand(TCommand command);
        protected abstract Task ExecuteCommandInTransaction(TCommand command);
    }
}
