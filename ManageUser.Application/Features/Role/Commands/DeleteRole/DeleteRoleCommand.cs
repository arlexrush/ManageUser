using ManageUser.Application.CQRSAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.DeleteRole
{
    public class DeleteRoleCommand : ICommandWResult<DeleteRoleCommandResponse>
    {
        public string Id { get; set; } = default!;
    }
}
