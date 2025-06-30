using ManageUser.Application.NotificationServices;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ManageUser.Application.CQRSAbstractions.DomainEventPublisher.Event
{
    public class UserUpdatedEventHandler : IDomainEventHandler<UserUpdatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public UserUpdatedEventHandler(IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "Unit of Work cannot be null.");
        }

        public async Task HandleAsync(UserUpdatedEvent domainEvent)
        {
            string toEmail = domainEvent.Email;
            string subject = "Welcome to ManageUser!";
            string htmlContent = $@"
                <h1>Welcome to ManageUser!</h1>
                <p>Dear User,</p>
                <p>Thank you for registering with ManageUser. Your user ID is: {domainEvent.UserId}.</p>
                <p>We are excited to have you on board!</p>
                <p>Best regards,<br/>ManageUser Team</p>";

            await _emailService.SendEmailAsync(toEmail, subject, htmlContent);

            await _unitOfWork.Repository<StoredDomainEvent>().AddAsync(new StoredDomainEvent
            {
                Id = Guid.NewGuid().ToString(),
                UserId = domainEvent.UserId,
                EventType = nameof(domainEvent),
                CreatedAt = DateTime.UtcNow,
                IsPublished = false,
                EventData = System.Text.Json.JsonSerializer.Serialize(domainEvent)
            });
            //guardar el evento de Dominio
            await _unitOfWork.EventRepository<UserUpdatedEvent>().AddAsync(domainEvent);
        }
    }
}
