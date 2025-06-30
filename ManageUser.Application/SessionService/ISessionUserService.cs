using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.SessionService
{
    public interface ISessionUserService
    {
        public string? GetUserId();
        public Task InvalidateRefreshTokenAsync(string userId, string refreshToken);
        public Task ClearSessionAsync(string userId);
    }
}
