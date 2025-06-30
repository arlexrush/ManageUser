using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using ManageUser.Domain;
using ManageUser.Domain.Address;
using ManageUser.Domain.Events;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManageUser.Infrastructure.EntityPersistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IDomainEventPublisher _domainEventPublisher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventPublisher domainEventPublisher) : base(options)
        {
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher), "DomainEventPublisher cannot be null.");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configuración de la tabla de usuarios
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.AvatarUrl).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.Users)
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(entity=>entity.Address)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // Configuración de la tabla de roles
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            // Configuración de la tabla de inquilinos (tenants)
            builder.Entity<Tenant<ApplicationUser>>(entity =>
            {
                entity.ToTable("Tenants");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.LogoUrl).HasMaxLength(200);
                entity.HasMany(e => e.Users)
                    .WithOne(t => t.Tenant)
                    .HasForeignKey(t => t.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de eventos de dominio
            builder.Entity<StoredDomainEvent>(entity =>
            {
                entity.ToTable("DomainEvents");
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EventData).IsRequired();
                entity.Property(e => e.OccurredOn).IsRequired();
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.UserId).HasMaxLength(450).IsRequired(false); // Assuming UserId is a string, adjust as necessary
            });

            builder.Entity<Invitation>(entity =>
            {
                entity.ToTable("Invitations");
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Expiration).IsRequired();
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.TenantId).IsRequired();

            });

            builder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
                entity.Property(e => e.Street).HasMaxLength(100);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.State).HasMaxLength(50);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.HasOne(e => e.Country)
                    .WithMany(c=>c.Addressess)
                    .HasForeignKey(e => e.Cca2Address)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<Country>(entity =>
            {
                entity.ToTable("Countries");
                entity.Property(e => e.cca2).IsRequired().HasMaxLength(2);
                entity.HasOne(e=>e.name)
                    .WithOne(n => n.country)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.name)
                    .WithOne(i => i.country)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.idd)
                    .WithOne(i => i.Country)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.currencies)
                    .WithOne(c => c.Country)
                    .HasForeignKey(c => c.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<NameCountry>(entity =>
            {
                entity.ToTable("CountryStates");
                entity.Property(e => e.common).IsRequired().HasMaxLength(100);
                entity.Property(e => e.official).IsRequired().HasMaxLength(100);
                entity.HasMany(c => c.nativeNames)
                    .WithOne(e => e.nameCountry)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Idd>(builder =>
            {
                builder.ToTable("Idds");
                builder.Property(e => e.root).IsRequired().HasMaxLength(10);
                builder.HasOne(e => e.Country)
                    .WithOne(s => s.idd)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Currency>(builder =>
            {
                builder.ToTable("Currencies");
                builder.Property(e => e.name).IsRequired().HasMaxLength(50);
                builder.Property(e => e.symbol).HasMaxLength(10);
                builder.HasOne(c => c.Country)
                    .WithMany(c => c.currencies)
                    .HasForeignKey(c => c.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // ManageUser.Infrastructure/Persistence/ApplicationDbContext.cs
        
        public DbSet<UserRegisteredEvent> UserRegisteredEvents { get; set; }
        public DbSet<UserUpdatedEvent> UserUpdatedEvents { get; set; }
        public DbSet<StoredDomainEvent> DomainEvents { get; set; }
        public DbSet<Tenant<ApplicationUser>> Tenants { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<NameCountry> CountryStates { get; set; }
        public DbSet<NativeName> NativeNames { get; set; }
        public DbSet<Currency> Currencies { get; set; }        
        public DbSet<Idd> Idds { get; set; }
        
    }
}
