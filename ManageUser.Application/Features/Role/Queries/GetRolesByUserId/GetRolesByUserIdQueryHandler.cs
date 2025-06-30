using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.JWTService;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.Features.Role.Queries.GetRolesByUserId
{
    public class GetRolesByUserIdQueryHandler : IQueryHandler<GetRolesByUserIdQuery, GetRolesByUserIdQueryResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public GetRolesByUserIdQueryHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<GetRolesByUserIdQueryResponse> HandleAsync(GetRolesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var userId = _jwtService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
                return new GetRolesByUserIdQueryResponse();

            var roles = await _userManager.GetRolesAsync(user);
            return new GetRolesByUserIdQueryResponse
            {
                Roles = roles.ToList()
            };
        }
    }
}
