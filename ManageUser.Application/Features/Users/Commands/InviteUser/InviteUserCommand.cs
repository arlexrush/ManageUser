using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.InviteUser
{
    public class InviteUserCommand : ICommandWResult<InviteUserCommandResponse>
    {
        public string Email { get; set; }
        public string TenantId { get; set; }
    }
}
