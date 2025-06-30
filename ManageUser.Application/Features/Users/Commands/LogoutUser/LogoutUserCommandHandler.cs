using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.SessionService;

namespace ManageUser.Application.Features.Users.Commands.LogoutUser
{
    public class LogoutUserCommandHandler : ICommandHandler<LogoutUserCommand>
    {
        private readonly ISessionUserService _sessionUserService;

        public LogoutUserCommandHandler(ISessionUserService sessionUserService)
        {
            _sessionUserService = sessionUserService;
        }
        public async Task HandleAsync(LogoutUserCommand command, CancellationToken cancellationToken)
        {
            // Invalida el refresh token (si aplica)
            await _sessionUserService.InvalidateRefreshTokenAsync(command.UserId, command.RefreshToken);

            // Limpia cualquier información de sesión adicional
            await _sessionUserService.ClearSessionAsync(command.UserId);
        }
    }
}
