using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions
{
    public interface IMediator
    {
        // Query
        Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>;

        // Command
        Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;

        // Command with Result
        Task<TResult> SendCommandWRAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommandWResult<TResult>;
    }
}
