using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.Features.Users.Commands.RegisterUser;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Users.Commands.RegisterInvitedUser
{
    public class RegisterInvitedUserCommand : ICommandWResult<RegisterUserCommandResponse>
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? TypeUser { get; set; } // Individual o Corporativo o Invitado
        public IFormFile ImageUser { get; set; }
        public string ImageUserId { get; set; }
        public string ImageUserUrl { get; set; }
        public string Password { get; set; }
        public string Address { get; set; } // Dirección del inquilino (tenant) al que pertenece el usuario
        public string CIF { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario
    }
}
