using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ManageUser.Application.Features.Users.Commands.AddClaimsToRole
{
    public class AddClaimToRoleCommandHandler : ICommandWResultHandler<AddClaimToRoleCommand, AddClaimToRoleCommandResponse>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AddClaimToRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<AddClaimToRoleCommandResponse> HandleAsync(AddClaimToRoleCommand command, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(command.RoleId);
            if (role == null)
                throw new BadRequestException("Rol no encontrado.");

            var claim = new Claim(command.ClaimType, command.ClaimValue);

            var result = await _roleManager.AddClaimAsync(role, claim);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return new AddClaimToRoleCommandResponse
            {
                RoleId = role.Id,
                ClaimType = command.ClaimType,
                ClaimValue = command.ClaimValue
            };
        }
    }
}
