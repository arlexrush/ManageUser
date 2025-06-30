using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.JWTService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Users.Commands.RemoveRoleFromUser
{
    public class RemoveRoleFromUserCommandHandler : ICommandWResultHandler<RemoveRoleFromUserCommand, RemoveRoleFromUserCommandResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public RemoveRoleFromUserCommandHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<RemoveRoleFromUserCommandResponse> HandleAsync(RemoveRoleFromUserCommand command, CancellationToken cancellationToken)
        {
            var userid = _jwtService.GetUserId();
            var user = await _userManager.FindByIdAsync(userid!);
            if (user == null)
                return new RemoveRoleFromUserCommandResponse { Success = false, Message = "Usuario no encontrado" };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count <= 1)
                return new RemoveRoleFromUserCommandResponse { Success = false, Message = "El usuario debe tener al menos un rol" };

            if (!roles.Contains(command.RoleName))
                return new RemoveRoleFromUserCommandResponse { Success = false, Message = "El usuario no tiene ese rol" };

            var result = await _userManager.RemoveFromRoleAsync(user, command.RoleName);
            if (!result.Succeeded)
                return new RemoveRoleFromUserCommandResponse { Success = false, Message = "No se pudo remover el rol" };

            return new RemoveRoleFromUserCommandResponse { Success = true, Message = "Rol removido correctamente" };
        }
    }
}
