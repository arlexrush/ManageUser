using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Features.Users.Commands.RegisterUser;
using ManageUser.Application.Repositories;
using ManageUser.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Application.Features.Users.Commands.RegisterInvitedUser
{
    public class RegisterInvitedUserCommandHandler : ICommandWResultHandler<RegisterInvitedUserCommand, RegisterUserCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterInvitedUserCommandHandler(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<RegisterUserCommandResponse> HandleAsync(RegisterInvitedUserCommand command, CancellationToken cancellationToken)
        {
            // Buscar invitación válida
            var invitations = await _unitOfWork.Repository<Invitation>().GetAsync(i => i.Token == command.Token && !i.Accepted && i.Expiration > DateTime.UtcNow);
            var invitation = invitations.FirstOrDefault();
            if (invitation == null)
                return new RegisterUserCommandResponse { IsSuccess = false };

            // Verificar que el usuario no exista
            var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
            if (existingUser != null)
                return new RegisterUserCommandResponse { IsSuccess = false };

            var tenant = await _unitOfWork.Repository<Tenant<ApplicationUser>>().GetByIdAsync(invitation.TenantId);

            // Crear usuario con rol "Guest"
            var user = new ApplicationUser
            {
                UserName = invitation.Email,
                Email = invitation.Email,
                FirstName = command.Name,
                TenantId = invitation.TenantId,
                Tenant = tenant,
                IsActive = true,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
                return new RegisterUserCommandResponse { IsSuccess = false };

            await _userManager.AddToRoleAsync(user, "Guest");

            // Marcar invitación como aceptada
            invitation.Accepted = true;
            await _unitOfWork.Repository<Invitation>().UpdateAsync(invitation);
            await _unitOfWork.Complete();

            return new RegisterUserCommandResponse { IsSuccess = true };
        }
    }
}

