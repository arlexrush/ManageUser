using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler : ICommandWResultHandler<DeleteRoleCommand, DeleteRoleCommandResponse>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<DeleteRoleCommandResponse> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(command.Id);
            if (role == null)
            {
                return new DeleteRoleCommandResponse
                {
                    Success = false,
                    Message = "Rol no encontrado"
                };
            }

            var result = await _roleManager.DeleteAsync(role);

            return new DeleteRoleCommandResponse
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "Rol eliminado correctamente" : string.Join("; ", result.Errors.Select(e => e.Description))
            };

        }
    }
}
