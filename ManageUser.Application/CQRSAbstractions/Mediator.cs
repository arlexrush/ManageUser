using ManageUser.Application.CQRSAbstractions.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Command
        public async Task SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            //await handler.HandleAsync(command);
            await ExecutivePipelines<TCommand,object?>(command, async _ => { await handler.HandleAsync(command, cancellationToken);return null; });
        }

        // Command with Result
        public async Task<TResult> SendCommandWRAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommandWResult<TResult>
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var handler = _serviceProvider.GetRequiredService<ICommandWResultHandler<TCommand, TResult>>();
            //return await handler.HandleAsync(command);
            return await ExecutivePipelines<TCommand, TResult>(command, async _ => await handler.HandleAsync(command, cancellationToken));
        }

        // Query
        public async Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            //return await handler.HandleAsync(query);
            return await ExecutivePipelines<TQuery, TResult>(query, async _ => await handler.HandleAsync(query, cancellationToken));
        }


        private async Task<TResponse> ExecutivePipelines<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> handlerFunc) where TRequest : notnull
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse().ToList();

            RequestHandlerDelegate<TResponse> next = () => handlerFunc(request);
            foreach (var behavior in behaviors)
            {
                var currentNext = next;
                next = () => behavior.Handle(request, currentNext, CancellationToken.None);
            }

            return await next();
        }
    }   
}
