using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.JWTService;
using ManageUser.Application.JWTService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ManageUser.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler : ICommandWResultHandler<LoginUserCommand, LoginUserCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ConfigToken _configTokenOptions;

        public LoginUserCommandHandler(IOptions<ConfigToken> configTokenOption, UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _configTokenOptions = configTokenOption.Value;
        }

        public async Task<LoginUserCommandResponse> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, command.Password))
                {
                    return new LoginUserCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Credenciales inválidas."
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Generar el JWT y obtener expiración
                var tokenResult = await _jwtService.GenerateTokenAsync(user.Id, user.Email!, roles);

                // Si manejas refresh token, aquí lo generas y guardas
                string? refreshToken = null;
                DateTime? refreshTokenExpiration = null;
                if (user.RefreshToken == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    refreshToken = Guid.NewGuid().ToString("N");
                    refreshTokenExpiration = DateTime.UtcNow.AddDays(_configTokenOptions.RefreshTokenDurationInDays); // O usa tu configuración
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = refreshTokenExpiration;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    refreshToken = user.RefreshToken;
                    refreshTokenExpiration = user.RefreshTokenExpiryTime;
                }

                return new LoginUserCommandResponse
                {
                    IsSuccess = true,
                    Token = tokenResult,
                    Expiration = DateTime.UtcNow.AddMinutes(_configTokenOptions.DurationInMinutes),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                // Manejo de excepciones, puedes registrar el error o lanzar una excepción personalizada
                return new LoginUserCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Ocurrió un error al procesar la solicitud."
                };
            }
            
        }
    }
}
