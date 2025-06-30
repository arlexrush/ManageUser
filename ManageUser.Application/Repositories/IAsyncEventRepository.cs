using ManageUser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Repositories
{
    public interface IAsyncEventRepository<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        Task AddAsync(TDomainEvent domainEvent);
    }
}
