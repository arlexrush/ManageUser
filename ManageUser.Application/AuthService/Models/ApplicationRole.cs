using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.AuthService.Models
{
    public class ApplicationRole : IdentityRole
    {              
        public string? Description { get; set; }

        // BaseEntity properties  
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string? CreatedBy { get; set; } // Usuario que creó la entidad
        public string? UpdatedBy { get; set; } // Usuario que actualizó la entidad
        

        // Constructor  
        public ApplicationRole(string name) : base()
        {
            Id = Guid.NewGuid().ToString(); // Generar un GUID como identificador único
            Name = name;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // BaseEntity method  
        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetCreatedAt()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
