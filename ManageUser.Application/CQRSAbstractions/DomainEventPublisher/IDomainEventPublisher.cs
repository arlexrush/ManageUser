using ManageUser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.CQRSAbstractions.DomainEventPublisher
{
    public interface IDomainEventPublisher
    {
        /// <summary>
        /// Publishes a domain event to all registered handlers.
        /// </summary>
        /// <param name="domainEvent">The domain event to publish.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent;
        /// <summary>
        /// Publishes a domain event synchronously to all registered handlers.
        /// </summary>
        /// <param name="domainEvent">The domain event to publish.</param>
        public void Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent;

        public Task SaveDomainEventsAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent;

        public Task PublishEventToMessageBrokerAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent;
    }
}
