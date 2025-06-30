using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.UpdateRole
{
    public class UpdateRoleCommandResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
