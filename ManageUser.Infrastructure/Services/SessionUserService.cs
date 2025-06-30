using ManageUser.Application.AuthService.Models;
using ManageUser.Application.SessionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Infrastructure.Services
{
    public class SessionUserService : ISessionUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager), "User Manager cannot be null.");
        }
        public string? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
                return null;

            var userId= user.FindFirst("sub")!.Value
                ?? user.FindFirst("id")?.Value
                ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            return userId;
        }

        public async Task InvalidateRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            // Solo invalida si el refreshToken coincide
            if (user.RefreshToken == refreshToken)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task ClearSessionAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            // Limpia cualquier información de sesión relevante
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }
    }
}
