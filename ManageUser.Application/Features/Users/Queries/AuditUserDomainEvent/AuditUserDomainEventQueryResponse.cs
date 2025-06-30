namespace ManageUser.Application.Features.Users.Queries.AuditUserDomainEvent
{
    public class AuditUserDomainEventQueryResponse
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
    }
}
