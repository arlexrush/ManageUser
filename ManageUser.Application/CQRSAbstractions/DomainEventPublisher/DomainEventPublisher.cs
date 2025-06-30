using ManageUser.Application.IntegrationEvents;
using ManageUser.Application.Repositories;
using ManageUser.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ManageUser.Application.CQRSAbstractions.DomainEventPublisher
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IExternalEventPublisher _externalEventPublisher;
        private readonly IServiceProvider? _serviceProvider;
        private readonly ILogger<DomainEventPublisher>? _logger;
        private readonly IUnitOfWork? _unitOfWork;

        public DomainEventPublisher(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, ILogger<DomainEventPublisher> logger, IExternalEventPublisher externalEventPublisher)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "Unit of Work cannot be null.");
            _externalEventPublisher = externalEventPublisher ?? throw new ArgumentNullException(nameof(externalEventPublisher), "External event publisher cannot be null.");
        }

        public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

            var handlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IDomainEventHandler<TDomainEvent>)); // El tipo de handler que estoy buiscando
            var handlers = (IEnumerable<object>?)_serviceProvider!.GetService(handlerType); // Obtengo los handlers del service provider

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    var handleMethod = handler.GetType().GetMethod("HandleAddEventDomainAsync"); // recorro la lista de los Handlers solicitados y busco el metodo "HandleAddEventDomainAsync" 
                    if (handleMethod != null)
                    {
                        var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent })!; // Invoco el metodo HandleAsync de cada handler
                        await task.ConfigureAwait(false);
                    }
                }
            }
        }

        public void Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
        {
            if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

            var handlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IDomainEventHandler<TDomainEvent>));
            var handlers = (IEnumerable<object>?)_serviceProvider!.GetService(handlerType);

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    var handleMethod = handler.GetType().GetMethod("HandleAsync");
                    if (handleMethod != null)
                    {
                        var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent })!;
                        task.GetAwaiter().GetResult();
                    }
                }
            }
        }

        public async Task SaveDomainEventsAsync<TDomainEvent>(TDomainEvent domainEvent,  CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
        {
            if (_unitOfWork == null)
            {
                _logger?.LogError("Unit of Work is not initialized.");
                throw new InvalidOperationException("Unit of Work is not initialized.");
            }
            try
            {
                await _unitOfWork.EventRepository<TDomainEvent>().AddAsync(domainEvent);
                await _unitOfWork.Complete(); 
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while saving domain events.");
                throw;
            }
        }

        public async Task PublishEventToMessageBrokerAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : IDomainEvent
        {
            // Publica en Kafka (topic basado en el nombre del evento)
            var topic = domainEvent.EventName.ToLowerInvariant(); // Ej: "userregisteredevent"
            await _externalEventPublisher.PublishAsync(domainEvent, topic, cancellationToken);
        }
    }
}
