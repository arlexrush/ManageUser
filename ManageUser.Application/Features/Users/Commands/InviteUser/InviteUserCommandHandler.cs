using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.NotificationServices;
using ManageUser.Application.Repositories;
using ManageUser.Application.SessionService;
using ManageUser.Domain;

namespace ManageUser.Application.Features.Users.Commands.InviteUser
{
    public class InviteUserCommandHandler : ICommandWResultHandler<InviteUserCommand, InviteUserCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ISessionUserService _sessionUserService;

        public InviteUserCommandHandler(IUnitOfWork unitOfWork, IEmailService emailService, ISessionUserService sessionUserService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _sessionUserService = sessionUserService;
        }

        public async Task<InviteUserCommandResponse> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken)
        {
            var token = Guid.NewGuid().ToString("N");
            var invitation = new Invitation
            {
                Email = command.Email,
                TenantId = command.TenantId,
                InvitedByUserId = _sessionUserService.GetUserId()!,
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(3),
                Accepted = false
            };
            await _unitOfWork.Repository<Invitation>().AddAsync(invitation);
            await _unitOfWork.Complete();

            var inviteUrl = $"https://tudominio.com/invite/accept?token={token}";
            var html = $"Has sido invitado a unirte como Guest. Haz clic aquí para registrarte: <a href='{inviteUrl}'>Aceptar invitación</a>";

            await _emailService.SendEmailAsync(command.Email, "Invitación a la plataforma", html);

            return new InviteUserCommandResponse { IsSuccess = true, Message = "Invitación enviada." };
        }

    }
}
