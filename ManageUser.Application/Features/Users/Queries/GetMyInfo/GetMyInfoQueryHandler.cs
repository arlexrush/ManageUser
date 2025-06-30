using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Exceptions;
using ManageUser.Application.JWTService;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Users.Queries.GetMyInfo
{
    public class GetMyInfoQueryHandler : IQueryHandler<GetMyInfoQuery, GetMyInfoQueryResponse>
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetMyInfoQueryHandler(IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService), "JWT Service cannot be null.");
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager), "User Manager cannot be null.");
        }

        public async Task<GetMyInfoQueryResponse> HandleAsync(GetMyInfoQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query), "Query cannot be null.");
            var userId = _jwtService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new JwtServiceException("User ID cannot be null or empty.");
            
            var userEmail = _jwtService.GetUserEmail();
            if (userEmail is null)
                throw new JwtServiceException("User not found.");

            var userRoles = _jwtService.GetUserRoles();
            if (userRoles.Count==0 || !userRoles.Any())
                throw new JwtServiceException("User roles cannot be null or empty.");
            
            var user = await _userManager.FindByIdAsync(userId);

            return new GetMyInfoQueryResponse
            {
                UserName = user!.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber!,
                Email = userEmail,
                Roles = userRoles
            };
        }
    }
}
