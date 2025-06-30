using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.IntegrationEvents
{
    public interface IExternalEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken);
    }

}
