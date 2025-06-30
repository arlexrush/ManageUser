using ManageUser.Application.NotificationServices;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Events;

namespace ManageUser.Application.CQRSAbstractions.DomainEventPublisher.Event
{
    public class UserRegisteredEventHandler : IDomainEventHandler<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public UserRegisteredEventHandler(IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork), "Unit of Work cannot be null.");
        }

        public async Task HandleAsync(UserRegisteredEvent @event)
        {
            string toEmail = @event.Email;
            string subject = "Welcome to ManageUser!";
            string htmlContent = $@"
                <h1>Welcome to ManageUser!</h1>
                <p>Dear User,</p>
                <p>Thank you for registering with ManageUser. Your user ID is: {@event.UserId}.</p>
                <p>We are excited to have you on board!</p>
                <p>Best regards,<br/>ManageUser Team</p>";

            await _emailService.SendEmailAsync(toEmail, subject, htmlContent);

            //guardar el evento de Dominio
            await _unitOfWork.EventRepository<UserRegisteredEvent>().AddAsync(@event);
            await _unitOfWork.Repository<StoredDomainEvent>().AddAsync(new StoredDomainEvent
            {
                 Id= Guid.NewGuid().ToString(),
                UserId = @event.UserId,
                EventType = nameof(@event),
                CreatedAt = DateTime.UtcNow,
                IsPublished = false,
                EventData = System.Text.Json.JsonSerializer.Serialize(@event)
            });
            
        }

        public async Task HandleAddEventDomainAsync(UserRegisteredEvent @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event), "Event cannot be null.");
            // Save the event to the repository
            await _unitOfWork.EventRepository<UserRegisteredEvent>().AddAsync(@event);



        }
    }
}
