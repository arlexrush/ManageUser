using ManageUser.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        public IAsyncRepository<TEntity> Repository<TEntity>() where TEntity : class;

        public IAsyncEventRepository<TDomainEvent>EventRepository<TDomainEvent>() where TDomainEvent : IDomainEvent;

        public Task<int> Complete();
        public void Dispose();
        
    }
}
