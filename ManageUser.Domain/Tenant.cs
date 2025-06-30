namespace ManageUser.Domain
{
    public class Tenant<TEntity>: BaseEntity<string>
    {
        public string Id { get; } = Guid.NewGuid().ToString(); // Identificador único del inquilino (tenant), se inicializa con un GUID    
        public string Name { get; set; } = string.Empty; // Nombre del inquilino (tenant)
        public string Description { get; set; } = string.Empty; // Descripción del inquilino (tenant)
        public string LogoUrl { get; set; } // URL de la imagen del inquilino (tenant)
        public string ImageTenantId { get; set; } // Identificador de la imagen del inquilino (tenant) en el almacenamiento
        public string? AddressTenant { get; set; } // Dirección del inquilino (tenant) al que pertenece el usuario
        public string? CIFTenant { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; // Indica si el inquilino está activo
        public bool IsDeleted { get; set; } = false; // Indica si el inquilino está eliminado
        // Relación con los usuarios
        public virtual ICollection<TEntity> Users { get; set; } = new List<TEntity>();
    }
    
}
