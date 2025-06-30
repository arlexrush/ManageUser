using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler : ICommandWResultHandler<UpdateRoleCommand, UpdateRoleCommandResponse>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UpdateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<UpdateRoleCommandResponse> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(command.Id);
            if (role == null)
            {
                return new UpdateRoleCommandResponse
                {
                    Success = false,
                    Message = "Rol no encontrado",
                    Id = command.Id,
                    Name = command.Name
                };
            }

            role.Name = command.Name;
            var result = await _roleManager.UpdateAsync(role);

            return new UpdateRoleCommandResponse
            {
                Id = role.Id,
                Name = role.Name,
                Success = result.Succeeded,
                Message = result.Succeeded ? "Rol actualizado correctamente" : string.Join("; ", result.Errors.Select(e => e.Description))
            };
        }
    }
}
