using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Role.Commands.CreateRole
{
    public class CreateRoleCommandHandler : ICommandWResultHandler<CreateRoleCommand, CreateRoleCommandResponse>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<CreateRoleCommandResponse> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
        {
            // Fix: Pass the required "name" parameter to the ApplicationRole constructor  
            var role = new ApplicationRole(command.Name)
            {
                Description = command.Description
            };

            try
            {
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    // Puedes lanzar una excepción personalizada o devolver un error según tu arquitectura  
                    throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return new CreateRoleCommandResponse
                {
                    Id = role.Id,
                    Name = role.Name!
                };
            }
            catch (Exception ex)
            {
                // Manejo de excepciones, puedes lanzar una excepción personalizada o registrar el error  
                throw new BadRequestException($"Error al crear el rol: {ex.Message}");

            }
        }
    }
}
