using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using ManageUser.Application.Repositories;
using ManageUser.Domain;
using ManageUser.Infrastructure.EntityPersistence;
using System.Collections;

namespace ManageUser.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        
        private Hashtable? _repositories;
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            
        }

        public async Task<int> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories is null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositoryBase<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            var response = (IAsyncRepository<TEntity>)_repositories[type]!;
            return response;
        }

        public IAsyncEventRepository<TDomainEvent> EventRepository<TDomainEvent>() where TDomainEvent : IDomainEvent
        {
            if (_repositories is null)
            {
                _repositories = new Hashtable();
            }
            var type = typeof(TDomainEvent).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(AsyncEventRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TDomainEvent)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            var response = (IAsyncEventRepository<TDomainEvent>)_repositories[type]!;
            return response;
        }
    }
}
