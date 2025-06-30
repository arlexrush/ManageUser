using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageUser.Domain
{
    public abstract class BaseEntity<TId>
    {
        // Identificador único de la entidad
        public TId Id { get; set; } // Se inicializa con un valor predeterminado no nulo

        // Fecha de creación
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Fecha de última actualización
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; } // Usuario que creó la entidad

        public string? UpdatedBy { get; set; } // Usuario que actualizó la entidad

        // Método para actualizar la fecha de creación

        public void SetCreatedAt()
        {
            CreatedAt = DateTime.UtcNow;
        }



        // Método para actualizar la fecha de modificación
        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // Constructor protegido para evitar instanciación directa
        protected BaseEntity() { }

        // Constructor para inicializar el identificador
        protected BaseEntity(TId id)
        {
            Id = id;
        }

        // Soporte para eventos de dominio
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


        // Método para agregar un evento de dominio
        protected internal void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        // Método para eliminar un evento de dominio
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}

