using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.NotificationServices;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandWResultHandler<ForgotPasswordCommand, ForgotPasswordCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ForgotPasswordCommandResponse> HandleAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null || !user.IsEmailConfirmed)
            {
                // No revelar si el usuario existe o no, por seguridad
                return new ForgotPasswordCommandResponse { IsSuccess = true };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://tudominio.com/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(user.Email, "Recupera tu contraseña",
                $"Para restablecer tu contraseña haz clic en el siguiente enlace: <a href='{resetLink}'>Restablecer contraseña</a>");

            return new ForgotPasswordCommandResponse { IsSuccess = true };
        }
    }
}
