using Confluent.Kafka;
using ManageUser.Application.AuthService.Models;
using ManageUser.Application.CountryService;
using ManageUser.Application.ImageServices;
using ManageUser.Application.IntegrationEvents;
using ManageUser.Application.JWTService;
using ManageUser.Application.JWTService.Models;
using ManageUser.Application.NotificationServices;
using ManageUser.Application.NotificationServices.Models;
using ManageUser.Application.Services;
using ManageUser.Application.SessionService;
using ManageUser.Infrastructure.EntityPersistence;
using ManageUser.Infrastructure.IntegrationEvents;
using ManageUser.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Text;

namespace ManageUser.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));

            // Configurar Entity Framework Core para PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    
                    // Habilitar seguimiento detallado en desarrollo
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                    {
                        options.EnableSensitiveDataLogging();
                    }
                });

           
            // Configurar ASP.NET Identity
            // Ensure the required package is installed: Microsoft.AspNetCore.Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Configuración de contraseñas
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Configuración de contraseñas                
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // Configuración de usuario
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // Cambiar a true si se requiere confirmación por email
            })
                .AddRoles<ApplicationRole>() // Añadir soporte para roles
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<ApplicationUser>>();

            // Configurar el servicio de correo electrónico
            services.AddTransient<IEmailService, EmailService>();

            // Validar la Configuración de ConfigToken
            services.AddOptions<ConfigToken>()
                .Bind(configuration.GetSection("ConfigToken"))
                .ValidateDataAnnotations();


            // Mapeo de ConfigToken desde appsettings.json a la clase ConfigToken
            services.Configure<ConfigToken>(configuration.GetSection("ConfigToken"));
            

            //Registrar el configurador de JwtBearerOptions
            services.AddTransient<IConfigureNamedOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

            // Configurar autenticación JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

            // Configuración de SendGrid
            services.Configure<SendGridSettings>(configuration.GetSection("SendGrid"));

            // Register JWT Service
            services.AddSingleton<IJwtService, JwtService>();

            
            // Ensure AddHttpContextAccessor is available
            services.AddHttpContextAccessor();
            services.AddScoped<ISessionUserService, SessionUserService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });


            // Configuración de Kafka usando appsettings.json y KafkaSettings
            services.AddSingleton<IProducer<string, string>>(sp =>
            {
                var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
                var kafkaConfig = new ProducerConfig
                {
                    BootstrapServers = kafkaSettings.BootstrapServers
                    // Agrega aquí otras propiedades si las defines en KafkaSettings
                };
                return new ProducerBuilder<string, string>(kafkaConfig).Build();
            });
            services.AddSingleton<IExternalEventPublisher, KafkaEventPublisher>();

            //Registrar el servicio de Gestion de Imagenes
            services.AddScoped<IManageImageService, ManageImageService>();

            services.AddScoped<ICountryService, CountryService>();

            return services;
        }
    }
}
