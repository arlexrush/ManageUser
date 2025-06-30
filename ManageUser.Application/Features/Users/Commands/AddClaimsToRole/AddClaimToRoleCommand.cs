using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.AddClaimsToRole
{
    public class AddClaimToRoleCommand : ICommandWResult<AddClaimToRoleCommandResponse>
    {
        public string RoleId { get; set; } = default!;
        public string ClaimType { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
    }
}
