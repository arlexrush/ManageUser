using ManageUser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions.DomainEventPublisher
{
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the domain event.
        /// </summary>
        /// <param name="event">The domain event to handle.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task  HandleAsync(TEvent @event);
    }
}
