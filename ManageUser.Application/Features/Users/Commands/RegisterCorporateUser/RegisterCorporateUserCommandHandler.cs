using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CQRSAbstractions;
using ManageUser.Application.CQRSAbstractions.DomainEventPublisher;
using ManageUser.Application.Features.Users.Commands.RegisterUser;
using ManageUser.Application.JWTService;
using ManageUser.Application.NotificationServices;
using ManageUser.Application.Repositories;
using ManageUser.Domain.Events;
using ManageUser.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ManageUser.Application.Features.Users.Commands.RegisterCorporateUser
{
    public class RegisterCorporateUserCommandHandler : ICommandWResultHandler<RegisterCorporateUserCommand, RegisterCorporateUserCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterCorporateUserCommandHandler(IUnitOfWork unitOfWork, 
            IJwtService jwtService, 
            ILogger<RegisterUserCommandHandler> logger, 
            IDomainEventPublisher domainEventPublisher, 
            IEmailService emailService, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<RegisterCorporateUserCommandResponse> HandleAsync(RegisterCorporateUserCommand command, CancellationToken cancellationToken)
        {

            // Validación de parámetros de entrada
            if (command == null) throw new ArgumentNullException(nameof(command), "Command cannot be null.");
            if (string.IsNullOrEmpty(command.Password))
            {
                _logger.LogError("Password cannot be null or empty.");
                throw new ArgumentException("Password cannot be null or empty.", nameof(command.Password));
            }
            if (string.IsNullOrEmpty(command.UserName))
            {
                _logger.LogError("Username cannot be null or empty.");
                throw new ArgumentException("Username cannot be null or empty.", nameof(command.UserName));
            }
            if (string.IsNullOrEmpty(command.Email))
            {
                _logger.LogError("Email cannot be null or empty.");
                throw new ArgumentException("Email cannot be null or empty.", nameof(command.Email));
            }
            if (string.IsNullOrEmpty(command.Name) || string.IsNullOrEmpty(command.LastName))
            {
                _logger.LogError("Name and LastName cannot be null or empty.");
                throw new ArgumentException("Name and LastName cannot be null or empty.");
            }
            if (command.ImageUser == null && string.IsNullOrEmpty(command.ImageUserId) && string.IsNullOrEmpty(command.ImageUserUrl))
            {
                _logger.LogError("ImageUser, ImageUserId, and ImageUserUrl cannot all be null or empty.");
                throw new ArgumentException("ImageUser, ImageUserId, and ImageUserUrl cannot all be null or empty.");
            }
            if (command.ImageUser != null && (string.IsNullOrEmpty(command.ImageUserId) || string.IsNullOrEmpty(command.ImageUserUrl)))
            {
                _logger.LogError("If ImageUser is provided, both ImageUserId and ImageUserUrl must be specified.");
                throw new ArgumentException("If ImageUser is provided, both ImageUserId and ImageUserUrl must be specified.");
            }


            // Estas lineas se ejecutan si el usuario es Corporativo, y requiere crear un nuevo Tenant
            var tenant = new Tenant<ApplicationUser>
            {
                Name = command.NameTenant!, // Default tenant name
                IsActive = true,
                IsDeleted = false,
                LogoUrl = command.ImageUserUrl, // Default logo URL
                ImageTenantId = command.ImageUserId, // Default image ID
                AddressTenant = command.AddressTenant, // Dirección del inquilino (tenant) al que pertenece el usuario
                CIFTenant = command.CIFTenant, // CIF del inquilino (tenant) al que pertenece el usuario
                Description = command.DescriptionTenant, // Descripción del inquilino (tenant) al que pertenece el usuario
                CreatedAt = DateTime.UtcNow,
            };
            try
            {
                // Create a new tenant if TenantId is not provided
                await _unitOfWork.Repository<Tenant<ApplicationUser>>().AddAsync(tenant);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant: {Message}", ex.Message);
                throw new InvalidOperationException("An error occurred while creating the tenant.", ex);
            }

            // Check if user already exists

            var existingUser = await _userManager.FindByNameAsync(command.UserName) ?? await _userManager.FindByEmailAsync(command.Email);
            if (existingUser != null)
            {
                _logger.LogError("User already exists with username or email.");
                throw new InvalidOperationException("User already exists with the provided username or email.");
            }


            // Crea el Usuario en memoria
            var user = new ApplicationUser
            {
                UserName = command.UserName,
                Email = command.Email,
                FirstName = command.Name,
                LastName = command.LastName,
                PhoneNumber = command.Phone,
                ImageUserId = command.ImageUserId,
                ImageUserUrl = command.ImageUserUrl,
                TenantId = tenant.Id!, // Assuming TenantId is part of the command
                Tenant = tenant, // Associate the user with the tenant
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                TypeUser = command.TypeUser!,
            };

            
            try
            {
                // Persistencia del nuevo usuario
                var result = await _userManager.CreateAsync(user, command.Password);

                //Gestion del Role
                if (!result.Succeeded)
                {
                    _logger.LogError("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new InvalidOperationException("User registration failed.");
                }
                // Assign role to user
                if (await _roleManager.RoleExistsAsync("Manager"))
                {
                    await _userManager.AddToRoleAsync(user, "Manager");
                }
                else
                {
                    // Create the role if it does not exist
                    var role = new IdentityRole("Manager");
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError("Role creation failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        throw new InvalidOperationException("Role creation failed.");
                    }
                    // Assign the newly created role to the user
                    await _userManager.AddToRoleAsync(user, "Manager");
                }
                await _unitOfWork.Complete();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Message}", ex.Message);
                throw new InvalidOperationException("An error occurred while registering the user.", ex);
            }
                    
            

            // Craete UserRegisteredEvent 
            var userRegisteredEvent = new UserRegisteredEvent(user.Id, user.Email);
            user.AddDomainEvent(userRegisteredEvent);


            // Generar token de confirmación de email
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // (Opcional) Enviar email de confirmación
            string confirmationLink = $"https://tudominio.com/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";
            // await _emailService.SendEmailAsync(user.Email, "Confirma tu correo", $"Haz clic aquí para confirmar: {confirmationLink}");


            string htmlContent = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #eee;padding:24px;background:#fafbfc;'>
                <div style='text-align:center;margin-bottom:24px;'>
                    <img src='{tenant.LogoUrl}' alt='{tenant.Name} Logo' style='max-width:180px;max-height:80px;margin-bottom:16px;' />
                    <h2 style='color:#2d3748;'>Bienvenido a {tenant.Name}</h2>
                </div>
                <p style='font-size:16px;color:#333;'>Hola <b>{user.FirstName} {user.LastName}</b>,</p>
                <p style='font-size:15px;color:#333;'>
                    Gracias por registrarte en <b>{tenant.Name}</b>.<br/>
                    Para completar tu registro y activar tu cuenta, por favor confirma tu correo electrónico haciendo clic en el siguiente botón:
                </p>
                <div style='text-align:center;margin:32px 0;'>
                    <a href='{confirmationLink}' style='background:#3182ce;color:#fff;padding:14px 32px;border-radius:6px;text-decoration:none;font-size:16px;display:inline-block;'>
                        Confirmar mi correo
                    </a>
                </div>
                <p style='font-size:14px;color:#666;'>
                    Si no creaste esta cuenta, puedes ignorar este mensaje.<br/>
                    <br/>
                    Saludos,<br/>
                    El equipo de {tenant.Name}
                </p>
                </div>
                ";

            await _emailService.SendEmailAsync(user.Email, $"Confirma tu correo en {tenant.Name}", htmlContent);


            // Generate JWT token
            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            var token = await _jwtService.GenerateTokenAsync(user.Id, user.Email, roles);

            // Persistir el Evento de creacion de Usuario          
            await _domainEventPublisher.SaveDomainEventsAsync(userRegisteredEvent, cancellationToken);

            // Publish the UserRegisteredEvent by Kafka
            await _domainEventPublisher.PublishEventToMessageBrokerAsync(userRegisteredEvent, cancellationToken);


            // Log the successful registration
            _logger.LogInformation("User {UserName} registered successfully with ID {UserId}.", user.UserName, user.Id);


            return new RegisterCorporateUserCommandResponse
            {
                IsSuccess = true,
            };

        }

    }
}
