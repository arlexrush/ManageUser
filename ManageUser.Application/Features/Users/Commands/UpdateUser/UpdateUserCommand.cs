using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : ICommandWResult<UpdateUserCommandResponse>
    {
        public string UserId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public IFormFile ImageUser { get; set; }
        public string ImageUserId { get; set; }
        public string ImageUserUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Street { get; set; } = default!;
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string Cca2Address { get; set; } = default!; // ISO 3166-1 alpha-2
        public string? CIF { get; set; }
    }
}
