namespace ManageUser.Domain.Events
{
    public class StoredDomainEvent
    {
        public string Id { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; } = string.Empty; // JSON serializado
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime OccurredOn { get; set; }
        public bool Processed { get; set; } = false;
        public DateTime ProcessedOn { get; set; }
        public string? UserId { get; set; }


        public StoredDomainEvent() { }
        protected StoredDomainEvent(string id, string eventType, string eventData, DateTime createdAt, bool isPublished, DateTime publishedAt, DateTime occurredOn, bool processed, DateTime processedOn)
        {
            Id = Guid.NewGuid().ToString();
            EventType = eventType;
            EventData = eventData;
            CreatedAt = DateTime.UtcNow;
            IsPublished = isPublished;
            PublishedAt = publishedAt;
            OccurredOn = DateTime.UtcNow;
            Processed = processed;
            ProcessedOn = processedOn;
        }
    }
}
