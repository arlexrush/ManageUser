using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Domain
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        
        /// Gets the type of the event.
        Type EventType { get; }

        /// Gets the unique identifier for the event.
        Guid EventId { get; }

        /// Gets the name of the event.
        string EventName { get; }
        
        /// Gets the version of the event.        
        int Version { get; }

        /// Gets the source of the event.
        string Source { get; }
        
        /// Gets the user who triggered the event.
        string TriggeredBy { get; }

        /// Gets the data associated with the event.
        object? Data { get; }
        
        /// Gets the metadata associated with the event.
        IDictionary<string, object> Metadata { get; }

        /// Gets the timestamp when the event was created.
        DateTime CreatedAt { get; }

        /// Gets the timestamp when the event was last modified.    
        DateTime? LastModifiedAt { get; }

        /// Gets the status of the event.
        string Status { get; }

        /// Gets the priority of the event.
        string Priority { get; }

        /// Gets the correlation identifier for the event.
        string CorrelationId { get; }

        /// Gets the source system that generated the event.
        string SourceSystem { get; }

        /// Gets the destination system for the event.
        string DestinationSystem { get; }

        /// Gets the event version.
        string EventVersion { get; }

        /// Gets the event category.
        string Category { get; }

        
    }
}
