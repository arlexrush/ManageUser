using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Users.Commands.RemoveRoleFromUser
{
    public class RemoveRoleFromUserCommand : ICommandWResult<RemoveRoleFromUserCommandResponse>
    {
        public string RoleName { get; set; }
    }
}
