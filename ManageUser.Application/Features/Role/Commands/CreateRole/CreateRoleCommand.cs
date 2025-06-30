using ManageUser.Application.CQRSAbstractions;

namespace ManageUser.Application.Features.Role.Commands.CreateRole
{
    public class CreateRoleCommand : ICommandWResult<CreateRoleCommandResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
