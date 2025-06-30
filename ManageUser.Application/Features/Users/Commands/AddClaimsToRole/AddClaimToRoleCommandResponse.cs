namespace ManageUser.Application.Features.Users.Commands.AddClaimsToRole
{
    public class AddClaimToRoleCommandResponse
    {
        public string RoleId { get; set; } = default!;
        public string ClaimType { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
    }
}
