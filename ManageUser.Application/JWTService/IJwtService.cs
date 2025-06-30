using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.JWTService
{
    public interface IJwtService
    {
        public Task<string> GenerateTokenAsync(string userId, string email, IEnumerable<string> roles);
        public ClaimsPrincipal? GetCurrentPrincipal();
        public bool HasClaim(string claimType, string? claimValue = null);
        public IEnumerable<Claim> GetAllClaims();
        public string? GetUserId();
        public string? GetUserEmail();
        public string? GetClaimValue(string claimType);
        public List<string> GetUserRoles();
        public string GetSessionUser();
        
    }
}
