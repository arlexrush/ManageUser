namespace ManageUser.Application.Features.Users.Queries.GetPagedUsers
{
    public class GetPagedUserQueryResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; } // Correo electrónico del usuario
        public string UserName { get; set; }
        public string PhoneNumber { get; set; } // Número de teléfono del usuario
        public string TypeUser { get; set; } // Tipo de usuario (Individual o Corporativo o Invitado)
        public string TenantId { get; set; } // Identificador del inquilino (tenant) al que pertenece el usuario
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; // Indica si el usuario está eliminado          
        public string? CIF { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
