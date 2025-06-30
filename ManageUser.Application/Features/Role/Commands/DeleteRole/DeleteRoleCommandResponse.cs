using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.DeleteRole
{
    public class DeleteRoleCommandResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
