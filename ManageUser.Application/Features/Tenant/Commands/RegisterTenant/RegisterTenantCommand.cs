using ManageUser.Application.CQRSAbstractions;
using Microsoft.AspNetCore.Http;

namespace ManageUser.Application.Features.Tenant.Commands.RegisterTenant
{
    public class RegisterTenantCommand : ICommandWResult<RegisterTenantCommandResponse>
    {
        public string Name { get; set; } // Nombre del inquilino (tenant)
        public string Description { get; set; } // Descripción del inquilino (opcional)
        public string LogoUrl { get; set; } // URL de la imagen del inquilino (tenant)
        public string ImageTenantId { get; set; } // Identificador de la imagen del inquilino (tenant) en el almacenamiento
        public IFormFile ImageUser { get; set; }
        public string ImageUserId { get; set; }
        public string ImageUserUrl { get; set; }
    }
}
