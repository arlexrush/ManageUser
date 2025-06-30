using ManageUser.Domain;
using ManageUser.Domain.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ManageUser.Application.AuthService.Models
{
    public class ApplicationUser : IdentityUser
    {       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string? Email { get; set; } // Correo electrónico del usuario
        public string TypeUser { get; set; } // Tipo de usuario (Individual o Corporativo o Invitado)
        public string AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false; // Indica si el usuario está eliminado  
        public bool IsEmailConfirmed { get; set; } = false; // Indica si el correo electrónico ha sido confirmado  
        public bool IsPhoneNumberConfirmed { get; set; } = false; // Indica si el número de teléfono ha sido confirmado  
        public IFormFile? ImageUser { get; set; }
        public string ImageUserId { get; set; }
        public string ImageUserUrl { get; set; }
        public virtual Address? Address { get; set; }

        public string? CIF { get; set; } // CIF del inquilino (tenant) al que pertenece el usuario


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; } // Usuario que creó la entidad  
        public string? UpdatedBy { get; set; } // Usuario que actualizó la entidad  
        public string? RefreshToken { get; set; } // Token de actualización  
        public DateTime? RefreshTokenExpiryTime { get; set; } // Fecha de expiración del token de actualización
                                                              // 

        // Relación con el inquilino (tenant)
        public string TenantId { get; set; } // Identificador del inquilino (tenant) al que pertenece el usuario  
        public virtual Tenant<ApplicationUser> Tenant { get; set; } // Relación con el inquilino (tenant) al que pertenece el usuario


        // Eventos de Dominio (Domain Events)
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


        // Constructor  
        public ApplicationUser() : base()
        {
            Id = Guid.NewGuid().ToString(); // Generar un GUID como identificador único  
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
