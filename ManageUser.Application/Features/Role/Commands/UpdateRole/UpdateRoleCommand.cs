using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Role.Commands.UpdateRole
{
    public class UpdateRoleCommand : ICommandWResult<UpdateRoleCommandResponse>
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
