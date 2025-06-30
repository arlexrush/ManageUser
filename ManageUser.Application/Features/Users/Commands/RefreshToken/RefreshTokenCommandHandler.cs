using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.JWTService.Models;
using ManageUser.Application.JWTService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ManageUser.Application.Features.Users.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandWResultHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ConfigToken _configTokenOptions;

        public RefreshTokenCommandHandler(
            IOptions<ConfigToken> configTokenOption,
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _configTokenOptions = configTokenOption.Value;
        }

        public async Task<RefreshTokenCommandResponse> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user == null || user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    return new RefreshTokenCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Refresh token inválido o expirado."
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Generar nuevo JWT
                var tokenResult = await _jwtService.GenerateTokenAsync(user.Id, user.Email!, roles);

                // Rotar el refresh token (opcional pero recomendado)
                var newRefreshToken = Guid.NewGuid().ToString("N");
                var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(_configTokenOptions.RefreshTokenDurationInDays);
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = newRefreshTokenExpiration;
                await _userManager.UpdateAsync(user);

                return new RefreshTokenCommandResponse
                {
                    IsSuccess = true,
                    Token = tokenResult,
                    Expiration = DateTime.UtcNow.AddMinutes(_configTokenOptions.DurationInMinutes),
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiration = newRefreshTokenExpiration,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            catch
            {
                return new RefreshTokenCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Ocurrió un error al renovar el token."
                };
            }
        }
    }
}
