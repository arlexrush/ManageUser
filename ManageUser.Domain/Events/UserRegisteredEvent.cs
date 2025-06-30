namespace ManageUser.Domain.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public string UserId { get; }
        public string Email { get; } 
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public Type EventType { get; } = typeof(UserRegisteredEvent);

        public Guid EventId { get; } = Guid.NewGuid();

        public string EventName { get; } = nameof(UserRegisteredEvent);

        public int Version { get; } = 1;

        public string Source { get; } = "ManageUser.Domain";

        public string TriggeredBy { get; } = "System"; // This could be set to the user who triggered the event, if applicable.

        public object? Data { get; } // Changed from `object?` to `object` to match the interface definition.

        public IDictionary<string, object> Metadata => throw new NotImplementedException();

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public DateTime? LastModifiedAt { get; }

        public string Status { get; } = "New"; // Default status, can be changed based on processing logic.

        public string Priority { get; } = "Normal"; // Default priority, can be adjusted based on business rules.

        public string CorrelationId { get; } = Guid.NewGuid().ToString(); // Unique identifier for correlating events, can be set based on the context of the event.

        public string SourceSystem { get; } = "ManageUser"; // The system that generated the event, can be set based on the application context.

        public string DestinationSystem { get; } = "ManageUser"; // The system that will consume the event, can be set based on the architecture of the application.

        public string EventVersion { get; } = "1.0"; // Version of the event, can be incremented based on changes in the event structure or business logic.

        public string Category { get; } = "UserManagement"; // Category of the event, can be used for filtering or routing events in an event-driven architecture.

        public UserRegisteredEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

    }
}
