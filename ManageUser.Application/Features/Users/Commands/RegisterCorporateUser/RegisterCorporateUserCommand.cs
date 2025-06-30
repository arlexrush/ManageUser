using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Users.Commands.RegisterCorporateUser
{
    public class RegisterCorporateUserCommand : ICommandWResult<RegisterCorporateUserCommandResponse>
    {
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
        public string? NameTenant { get; set; } // Nombre del inquilino (tenant) al que pertenece el usuario
        public string? AddressTenant { get; set; } // Dirección del inquilino (tenant) al que pertenece el usuario
        public string? CIFTenant { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario
        public string DescriptionTenant { get; set; } // Descripción del inquilino (tenant) al que pertenece el usuario
    }
}
