using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.JWTService.Models;
using ManageUser.Application.JWTService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace ManageUser.Application.Features.Users.Commands.LoginUserWGmail
{
    public class GoogleLoginCommandHandler : ICommandWResultHandler<GoogleLoginCommand, GoogleLoginCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ConfigToken _configTokenOptions;

        public GoogleLoginCommandHandler(
            IOptions<ConfigToken> configTokenOption,
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _configTokenOptions = configTokenOption.Value;
        }

        public async Task<GoogleLoginCommandResponse> HandleAsync(GoogleLoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Validar el id_token de Google
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(command.IdToken);

                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return new GoogleLoginCommandResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "No se pudo obtener el email del token de Google."
                    };
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Crear usuario sin password, marcando el email como confirmado
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = name ?? "",
                        IsEmailConfirmed = true,
                        TypeUser = "Individual",
                        AvatarUrl = "",
                        IsDeleted = false
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, "Player");
                }

                var roles = await _userManager.GetRolesAsync(user);

                // Generar el JWT
                var tokenResult = await _jwtService.GenerateTokenAsync(user.Id, user.Email!, roles);

                // Refresh token
                string? refreshToken = null;
                DateTime? refreshTokenExpiration = null;
                if (user.RefreshToken == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    refreshToken = Guid.NewGuid().ToString("N");
                    refreshTokenExpiration = DateTime.UtcNow.AddDays(_configTokenOptions.RefreshTokenDurationInDays);
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = refreshTokenExpiration;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    refreshToken = user.RefreshToken;
                    refreshTokenExpiration = user.RefreshTokenExpiryTime;
                }

                return new GoogleLoginCommandResponse
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
            catch
            {
                return new GoogleLoginCommandResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Ocurrió un error al procesar el login con Google."
                };
            }
        }
    }
}
