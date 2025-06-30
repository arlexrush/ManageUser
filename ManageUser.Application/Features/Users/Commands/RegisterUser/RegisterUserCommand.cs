using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : ICommandWResult<RegisterUserCommandResponse>
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Country { get; set; } // País del usuario
        public string? TypeUser { get; set; } // Individual o Corporativo o Invitado
        public IFormFile ImageUser { get; set; }
        public string ImageUserId { get; set; }
        public string ImageUserUrl { get; set; }
        public string Password { get; set; }
        public string Street { get; set; } = default!;
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string Cca2Address { get; set; } = default!; // ISO 3166-1 alpha-2
        public string CIF { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario
    }
}
