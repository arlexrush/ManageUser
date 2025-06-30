using ManageUser.Application.Repositories;
using ManageUser.Domain;
using ManageUser.Domain.Events;
using ManageUser.Infrastructure.EntityPersistence;
using Newtonsoft.Json;

namespace ManageUser.Infrastructure.Repositories
{
    public class AsyncEventRepository<TDomainEvent> : IAsyncEventRepository<TDomainEvent>  where TDomainEvent : IDomainEvent
    {
        protected readonly ApplicationDbContext _context;

        // Constructor to initialize _context
        public AsyncEventRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAsync(TDomainEvent domainEvent)
        { 
            // Guardar evento
            var storedEvent = new StoredDomainEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventType = domainEvent.GetType().FullName!,
                EventData = JsonConvert.SerializeObject(domainEvent),
                OccurredOn = domainEvent.OccurredOn
            };
            _context.DomainEvents.Add(storedEvent);  
            await _context.SaveChangesAsync();
        }
    }
}
