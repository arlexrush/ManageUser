using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.NotificationServices;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Role.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandHandler : ICommandWResultHandler<ResendConfirmationEmailCommand, ResendConfirmationEmailCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ResendConfirmationEmailCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ResendConfirmationEmailCommandResponse> HandleAsync(ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            if (user == null)
            {
                return new ResendConfirmationEmailCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Usuario no encontrado."
                };
            }

            if (user.IsEmailConfirmed)
            {
                return new ResendConfirmationEmailCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "El correo ya está confirmado."
                };
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://tudominio.com/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(user.Email, "Confirma tu correo electrónico",
                $"Por favor confirma tu correo haciendo clic en el siguiente enlace: <a href='{confirmationLink}'>Confirmar Email</a>");

            return new ResendConfirmationEmailCommandResponse
            {
                IsSuccess = true
            };
        }
    }
}
