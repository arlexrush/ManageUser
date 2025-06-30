using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ICommandWResultHandler<ResetPasswordCommand, ResetPasswordCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResetPasswordCommandResponse> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return new ResetPasswordCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Usuario no encontrado."
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
            if (!result.Succeeded)
            {
                return new ResetPasswordCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "No se pudo restablecer la contraseña."
                };
            }

            return new ResetPasswordCommandResponse { IsSuccess = true };
        }
    }
}
