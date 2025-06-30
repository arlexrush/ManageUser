using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Events;

namespace ManageUser.Application.Features.Users.Queries.AuditUserDomainEvent
{
    public class AuditUserDomainEventQueryHandler : IQueryHandler<AuditUserDomainEventQuery, List<AuditUserDomainEventQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditUserDomainEventQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "Unit of Work cannot be null.");
        }
        public async Task<List<AuditUserDomainEventQueryResponse>> HandleAsync(AuditUserDomainEventQuery query, CancellationToken cancellationToken)
        {
            var auditEvents = (await _unitOfWork.Repository<StoredDomainEvent>().GetAllAsync()).ToList().Select(x=>new AuditUserDomainEventQueryResponse() {
                Id = x.Id,
                EventData = x.EventData,
                EventType = x.EventType,
                CreatedAt = x.CreatedAt,
                IsPublished = x.IsPublished,
                UserId = x.UserId
            }).ToList();

            return auditEvents;
        }
    }
}
